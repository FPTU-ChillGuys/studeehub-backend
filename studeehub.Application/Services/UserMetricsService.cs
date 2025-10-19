using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;
using System.Linq;

namespace studeehub.Application.Services
{
	public class UserMetricsService : IUserMetricsService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPomodoroSessionRepository _pomodoroSessionRepository;
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly IScheduleRepository _scheduleRepository;
		private readonly IStreakRepository _streakRepository;
		private readonly IGenericRepository<UserAchievement> _userAchievementRepository;

		public UserMetricsService(IPomodoroSessionRepository pomodoroSessionRepository, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository, IStreakRepository streakRepository, IGenericRepository<UserAchievement> userAchievementRepository, IUserRepository userRepository)
		{
			_pomodoroSessionRepository = pomodoroSessionRepository;
			_subscriptionRepository = subscriptionRepository;
			_scheduleRepository = scheduleRepository;
			_streakRepository = streakRepository;
			_userAchievementRepository = userAchievementRepository;
			_userRepository = userRepository;
		}

        public async Task<BaseResponse<UserMetricsResponse>> GetAdminMetricsAsync(GetUserMetricsRequest request)
        {
            var now = DateTime.UtcNow;
            var (start, end) = UserMetricsHelper.ResolveDateRange(request, now);

            // --- Load all users once ---
            var allUsers = await _userRepository.GetAllAsync(asNoTracking: true);
            if (!allUsers.Any())
                return BaseResponse<UserMetricsResponse>.Ok(new UserMetricsResponse(), "No users found.");

            // --- Filter only regular users (exclude admins) ---
            var userRoles = await _userRepository.GetUserRolesAsync(allUsers.Select(u => u.Id).ToList());
            var validUsers = allUsers
                .Where(u => userRoles.TryGetValue(u.Id, out var roles) && roles.Contains("USER"))
                .ToList();
            if (!validUsers.Any())
                return BaseResponse<UserMetricsResponse>.Ok(new UserMetricsResponse(), "No valid (non-admin) users found.");

            var validUserIds = validUsers.Select(u => u.Id).ToHashSet();

            // --- Overview ---
            int totalUsers = validUsers.Count();
            int activeUsers = validUsers.Count(u => u.IsActive);
            int inactiveUsers = totalUsers - activeUsers;

            // --- Growth ---
            var usersInRange = validUsers.Where(u => u.CreatedAt >= start && u.CreatedAt <= end).ToList();
            int newUsers = usersInRange.Count;

            var rangeDays = (end - start).TotalDays;
            var prevStart = start.AddDays(-rangeDays);
            var prevEnd = start.AddTicks(-1);
            int prevNewUsers = validUsers.Count(u => u.CreatedAt >= prevStart && u.CreatedAt <= prevEnd);

            double growthRatePercent = prevNewUsers == 0
                ? 0
                : Math.Round(((double)(newUsers - prevNewUsers) / prevNewUsers) * 100, 2);

            // --- Subscriptions ---
            var activeSubs = await _subscriptionRepository.GetAllAsync(
                s => s.Status == SubscriptionStatus.Active,
                include: s => s.Include(s => s.SubscriptionPlan),
                asNoTracking: true
            );

            var subsByPlan = activeSubs
                .Where(s => validUserIds.Contains(s.UserId))
                .GroupBy(s => s.SubscriptionPlan.Name ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            int totalSubscribers = subsByPlan.Values.Sum();
            double payingUserRatio = totalUsers == 0 ? 0 : Math.Round((double)totalSubscribers / totalUsers * 100, 2);

            // --- Productivity: Pomodoro sessions ---
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

            // --- Retention ---
            var recentSessions = await _pomodoroSessionRepository.GetAllAsync(
                s => validUserIds.Contains(s.UserId) && s.Start >= now.AddDays(-30),
                asNoTracking: true
            );

            var activeUserIds = recentSessions.Select(s => s.UserId).Distinct().ToList();
            int usersActive7Days = activeUserIds.Count(uid => recentSessions.Any(s => s.UserId == uid && s.Start >= now.AddDays(-7)));
            int usersActive30Days = activeUserIds.Count;

            double retention7DayPercent = totalUsers == 0 ? 0 : Math.Round((double)usersActive7Days / totalUsers * 100, 2);
            double retention30DayPercent = totalUsers == 0 ? 0 : Math.Round((double)usersActive30Days / totalUsers * 100, 2);

            // --- Growth Chart ---
            var groupBy = request.GroupBy?.ToLower() ?? "month";
            var monthlyGrowth = UserMetricsHelper.GroupUserGrowth(usersInRange, groupBy);

            // --- Trend Summary ---
            var trend = new UserTrendSummary
            {
                CurrentStart = start,
                CurrentEnd = end,
                PrevStart = prevStart,
                PrevEnd = prevEnd,
                CurrentNewUsers = newUsers,
                PrevNewUsers = prevNewUsers,
                GrowthRatePercent = growthRatePercent
            };

            // --- Build Response ---
            var response = new UserMetricsResponse
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                NewUsers = newUsers,
                GrowthRatePercent = growthRatePercent,
                PayingUserRatio = payingUserRatio,
                UsersBySubscription = subsByPlan,
                AvgSessionsPerUser = avgSessionsPerUser,
                TotalSchedules = totalSchedules,
                AvgSchedulesPerUser = avgSchedulesPerUser,
                TotalStreaks = totalStreaks,
                ActiveStreaks = activeStreaks,
                AvgStreakLength = avgStreakLength,
                TotalAchievementsUnlocked = totalAchievementsUnlocked,
                TopAchievements = topAchievements,
                Retention7DayPercent = retention7DayPercent,
                Retention30DayPercent = retention30DayPercent,
                MonthlyGrowth = monthlyGrowth,
                Trend = trend
            };

            return BaseResponse<UserMetricsResponse>.Ok(response, "User metrics retrieved successfully");
        }

        public async Task<BaseResponse<UserSelfMetricsResponse>> GetUserSelfMetricsAsync(
    Guid userId,
    GetUserSelfMetricsRequest request)
        {
            var now = DateTime.UtcNow;
            var (start, end) = UserMetricsHelper.ResolveDateRange(request.Period, now);

            var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
            if (user == null)
                return BaseResponse<UserSelfMetricsResponse>.Fail("User not found");

            // --- Pomodoro Sessions ---
            var sessions = await _pomodoroSessionRepository.GetAllAsync(
                s => s.UserId == userId && s.Start >= start && s.Start <= end,
                asNoTracking: true
            );

            int totalSessions = sessions.Count();
            double totalFocusHours = sessions.Sum(s => s.Duration.TotalHours);

            var totalDays = Math.Max(1, (end.Date - start.Date).TotalDays + 1);
            double avgFocusPerDay = Math.Round(totalFocusHours / totalDays, 2);

            var focusChart = sessions
                .Where(s => s.Start.HasValue)
                .GroupBy(s => s.Start.Value.Date)
                .Select(g => new DailyFocusSummary
                {
                    Date = g.Key,
                    FocusHours = Math.Round(g.Sum(x => x.Duration.TotalHours), 2)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // --- Schedules ---
            var schedules = await _scheduleRepository.GetAllAsync(s => s.UserId == userId, asNoTracking: true);
            int totalSchedules = schedules.Count();
            int completedSchedules = schedules.Count(s => s.IsCheckin);
            double completionRate = totalSchedules == 0
                ? 0
                : Math.Round((double)completedSchedules / totalSchedules * 100, 2);

            // --- Streaks ---
            var streaks = await _streakRepository.GetAllAsync(s => s.UserId == userId, asNoTracking: true);
            int currentStreak = streaks.OrderByDescending(s => s.LastUpdated).FirstOrDefault()?.CurrentCount ?? 0;
            int longestStreak = streaks.Any() ? streaks.Max(s => s.LongestCount) : 0;

            // --- Achievements ---
            var achievements = await _userAchievementRepository.GetAllAsync(
                a => a.UserId == userId,
                include: a => a.Include(x => x.Achievement),
                asNoTracking: true
            );

            int unlockedAchievements = achievements.Count();
            var recentAchievements = achievements
                .OrderByDescending(a => a.UnlockedAt)
                .Take(5)
                .Select(a => a.Achievement?.Name ?? "Unknown")
                .ToList();

            // --- Engagement ---
            int activeDays = focusChart.Count();
            double engagementRate = Math.Round((activeDays / totalDays) * 100, 2);

            // --- Subscription ---
            var subs = await _subscriptionRepository.GetAllAsync(
                s => s.UserId == userId && s.Status == SubscriptionStatus.Active,
                include: s => s.Include(x => x.SubscriptionPlan),
                asNoTracking: true
            );

            string subscriptionName = subs.FirstOrDefault()?.SubscriptionPlan?.Name ?? "Free";

            // --- Build Response ---
            var response = new UserSelfMetricsResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                SubscriptionName = subscriptionName,
                JoinedAt = user.CreatedAt,

                TotalSessions = totalSessions,
                TotalFocusHours = Math.Round(totalFocusHours, 2),
                AvgFocusPerDay = avgFocusPerDay,
                FocusChart = focusChart,

                TotalSchedules = totalSchedules,
                CompletedSchedules = completedSchedules,
                CompletionRate = completionRate,

                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,

                UnlockedAchievements = unlockedAchievements,
                RecentAchievements = recentAchievements,

                ActiveDays = activeDays,
                EngagementRatePercent = engagementRate
            };

            return BaseResponse<UserSelfMetricsResponse>.Ok(response, "User self metrics retrieved successfully");
        }
    }
}
