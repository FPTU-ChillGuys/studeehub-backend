using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;
using System.Globalization;

namespace studeehub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPomodoroSessionRepository _pomodoroSessionRepository;
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly IScheduleRepository _scheduleRepository;
		private readonly IStreakRepository _streakRepository;
		private readonly IGenericRepository<UserAchievement> _userAchievementRepository;
		private readonly IValidator<UpdateUserRequest> _updateUserValidator;
		private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, IMapper mapper, IPomodoroSessionRepository pomodoroSessionRepository, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository, IStreakRepository streakRepository, IGenericRepository<UserAchievement> userAchievementRepository, IValidator<UpdateUserRequest> updateUserValidator)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_pomodoroSessionRepository = pomodoroSessionRepository;
			_subscriptionRepository = subscriptionRepository;
			_scheduleRepository = scheduleRepository;
			_streakRepository = streakRepository;
			_userAchievementRepository = userAchievementRepository;
			_updateUserValidator = updateUserValidator;
		}

		public Task<bool> IsUserExistAsync(Guid userId)
		{
			return _userRepository.AnyAsync(u => u.Id == userId);
		}

		public Task<User?> GetUserByIdAsync(Guid userId)
		{
			return _userRepository.GetByConditionAsync(u => u.Id == userId);
		}

		public async Task<BaseResponse<GetUserResponse>> GetProfileByIdAsync(Guid userId)
		{
			var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (user == null)
			{
				return BaseResponse<GetUserResponse>.Fail("User not found", Domain.Enums.ErrorType.NotFound);
			}

			var response = _mapper.Map<GetUserResponse>(user);
			return BaseResponse<GetUserResponse>.Ok(response, "User profile retrieved successfully");
		}

		public async Task<BaseResponse<UserMetricsResponse>> GetUserMetricsAsync(GetUserMetricsRequest request)
		{
			var now = DateTime.UtcNow;
			var (start, end) = ResolveDateRange(request, now);

			// --- Load users once ---
			var allUsers = await _userRepository.GetAllAsync(asNoTracking: true);
			if (!allUsers.Any())
				return BaseResponse<UserMetricsResponse>.Ok(new UserMetricsResponse(), "No users found.");

			// --- Filter out admin/system accounts efficiently ---
			// Instead of parallel IsUserAsync(), use RoleRepository to fetch user roles in bulk.
			var userRoles = await _userRepository.GetUserRolesAsync(allUsers.Select(u => u.Id).ToList());
			var validUsers = allUsers
				.Where(u => userRoles.TryGetValue(u.Id, out var roles) && roles.Contains("USER"))
				.ToList();
			var validUserIds = validUsers.Select(u => u.Id).ToHashSet();

			if (!validUsers.Any())
				return BaseResponse<UserMetricsResponse>.Ok(new UserMetricsResponse(), "No valid (non-admin) users found.");

			// --- Basic totals ---
			int totalUsers = validUsers.Count;
			int activeUsers = validUsers.Count(u => u.IsActive);
			int inactiveUsers = totalUsers - activeUsers;

			// --- Users created in selected period ---
			var usersInRange = validUsers.Where(u => u.CreatedAt >= start && u.CreatedAt <= end).ToList();
			int newUsersCount = usersInRange.Count;

			// --- Previous period (for growth rate) ---
			var rangeLength = end - start;
			var prevStart = start.AddDays(-rangeLength.TotalDays);
			var prevEnd = start.AddSeconds(-1);
			int prevUsersCount = validUsers.Count(u => u.CreatedAt >= prevStart && u.CreatedAt <= prevEnd);

			double growthRate = prevUsersCount == 0
				? 0
				: Math.Round(((double)(newUsersCount - prevUsersCount) / prevUsersCount) * 100, 2);

			// --- Subscription breakdown ---
			var activeSubs = await _subscriptionRepository.GetAllAsync(
				s => s.Status == SubscriptionStatus.Active,
				include: s => s.Include(s => s.SubscriptionPlan),
				asNoTracking: true
			);

			var subsByPlan = activeSubs
				.Where(s => validUserIds.Contains(s.UserId))
				.GroupBy(s => s.SubscriptionPlan.Name ?? "Unknown")
				.ToDictionary(g => g.Key, g => g.Count());

			int totalSubscribers = subsByPlan.Sum(x => x.Value);
			double payingUserRatio = totalUsers == 0 ? 0 : Math.Round((double)totalSubscribers / totalUsers * 100, 2);

			// --- Engagement: Pomodoro sessions ---
			var totalSessions = await _pomodoroSessionRepository.CountAsync(s => validUserIds.Contains(s.UserId));
			double avgSessionsPerUser = totalUsers == 0 ? 0 : Math.Round((double)totalSessions / totalUsers, 2);

			// --- Schedules ---
			var totalSchedules = await _scheduleRepository.CountAsync(s => validUserIds.Contains(s.UserId));
			double avgSchedulesPerUser = totalUsers == 0 ? 0 : Math.Round((double)totalSchedules / totalUsers, 2);

			// --- Streaks ---
			var streaks = await _streakRepository.GetAllAsync(s => validUserIds.Contains(s.UserId));
			int totalStreaks = streaks.Count();
			int activeStreaks = streaks.Count(s => s.IsActive);
			double avgStreakLength = totalStreaks == 0 ? 0 : Math.Round(streaks.Average(s => s.LongestCount), 2);

			// --- Achievements ---
			var achievements = await _userAchievementRepository.GetAllAsync(
				a => validUserIds.Contains(a.UserId),
				include: a => a.Include(a => a.Achievement),
				asNoTracking: true
			);
			int totalAchievementsUnlocked = achievements.Count();
			var topAchievements = achievements
				.GroupBy(a => a.Achievement.Name ?? "Unknown")
				.OrderByDescending(g => g.Count())
				.Take(5)
				.ToDictionary(g => g.Key, g => g.Count());

			// --- Retention: users active in last 7 / 30 days ---
			var recentSessions = await _pomodoroSessionRepository.GetAllAsync(
				s => validUserIds.Contains(s.UserId) && s.Start >= now.AddDays(-30),
				asNoTracking: true
			);

			var usersActive7Days = recentSessions.Select(s => s.UserId).Distinct().Count(uid =>
				validUsers.Any(u => u.Id == uid && u.CreatedAt <= now.AddDays(-7)));

			var usersActive30Days = recentSessions.Select(s => s.UserId).Distinct().Count(uid =>
				validUsers.Any(u => u.Id == uid && u.CreatedAt <= now.AddDays(-30)));

			double retention7Day = totalUsers == 0 ? 0 : Math.Round((double)usersActive7Days / totalUsers * 100, 2);
			double retention30Day = totalUsers == 0 ? 0 : Math.Round((double)usersActive30Days / totalUsers * 100, 2);

			// --- Group for temporal chart ---
			var groupBy = request.GroupBy?.ToLower() ?? "month";
			var growthSeries = GroupUserGrowth(usersInRange, groupBy);

			// --- Build trend summary ---
			var trend = new UserTrendSummary
			{
				CurrentPeriodStart = start,
				CurrentPeriodEnd = end,
				PreviousPeriodStart = prevStart,
				PreviousPeriodEnd = prevEnd,
				CurrentNewUsers = newUsersCount,
				PreviousNewUsers = prevUsersCount,
				GrowthRatePercent = growthRate
			};

			// --- Build response ---
			var response = new UserMetricsResponse
			{
				TotalUsers = totalUsers,
				ActiveUsers = activeUsers,
				InactiveUsers = inactiveUsers,
				NewUsersCount = newUsersCount,
				GrowthRatePercent = growthRate,
				UsersBySubscription = subsByPlan,
				AverageSessionsPerUser = avgSessionsPerUser,
				TotalSchedules = totalSchedules,
				AverageSchedulesPerUser = avgSchedulesPerUser,
				TotalStreaks = totalStreaks,
				ActiveStreaks = activeStreaks,
				AverageStreakLength = avgStreakLength,
				TotalAchievementsUnlocked = totalAchievementsUnlocked,
				TopAchievements = topAchievements,
				MonthlyUserGrowth = growthSeries,
				TrendSummary = trend,
				PayingUserRatio = payingUserRatio,
				Retention7DayPercent = retention7Day,
				Retention30DayPercent = retention30Day
			};

			return BaseResponse<UserMetricsResponse>.Ok(response, "User metrics retrieved successfully");
		}

		private static (DateTime Start, DateTime End) ResolveDateRange(GetUserMetricsRequest request, DateTime now)
		{
			if (request.StartDate.HasValue && request.EndDate.HasValue)
			{
				var start = request.StartDate.Value.Date;
				var end = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
				return (start, end);
			}

			return request.Period?.ToLower() switch
			{
				"week" => GetWeekRange(now),
				"year" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
				_ => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1))
			};
		}

		private static (DateTime Start, DateTime End) GetWeekRange(DateTime date)
		{
			int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
			var start = date.AddDays(-diff).Date;
			var end = start.AddDays(7).AddTicks(-1);
			return (start, end);
		}

		private static List<MonthlyUserCount> GroupUserGrowth(IEnumerable<User> users, string groupBy)
		{
			return groupBy switch
			{
				"day" => users
					.GroupBy(u => u.CreatedAt.Date)
					.Select(g => new MonthlyUserCount
					{
						Year = g.Key.Year,
						Month = g.Key.Month,
						Count = g.Count()
					})
					.OrderBy(g => g.Year).ThenBy(g => g.Month).ToList(),

				"week" => users
					.GroupBy(u => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
						u.CreatedAt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
					.Select(g => new MonthlyUserCount
					{
						Year = users.Min(u => u.CreatedAt.Year),
						Week = g.Key,
						Count = g.Count()
					})
					.OrderBy(g => g.Year).ThenBy(g => g.Week).ToList(),

				_ => users
					.GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
					.Select(g => new MonthlyUserCount
					{
						Year = g.Key.Year,
						Month = g.Key.Month,
						Count = g.Count()
					})
					.OrderBy(g => g.Year).ThenBy(g => g.Month).ToList(),
			};
		}

		public async Task<BaseResponse<string>> UpdateProfileAsync(Guid userId, UpdateUserRequest request)
		{
			var validationResult = await _updateUserValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				return BaseResponse<string>.Fail("Validation errors: " + string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)), Domain.Enums.ErrorType.Validation);
			}
			var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (user == null)
			{
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);
			}
			_mapper.Map(request, user);
			_userRepository.Update(user);
			var result = await _userRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Profile updated successfully")
				: BaseResponse<string>.Fail("Failed to update profile", Domain.Enums.ErrorType.ServerError);
		}
	}
}
