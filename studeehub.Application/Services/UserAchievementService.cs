using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using studeehub.Application.DTOs.Requests.UserAchievem;
using studeehub.Application.DTOs.Responses.Achievement;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Achievements;
using studeehub.Domain.Enums.Streaks;
using studeehub.Infrastructure.Extensions;

namespace studeehub.Application.Services
{
	public class UserAchievementService : IUserAchievementService
	{
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly IUserService _userService;
		private readonly IMapper _mapper;
		private readonly IValidator<UnlockAchivemRequest> _unlockValidator;
		private readonly IGenericRepository<UserAchievement> _userAchievementRepository;
		private readonly IGenericRepository<Achievement> _achievementRepository;

		public UserAchievementService(
			IUserService userService,
			IMapper mapper,
			IValidator<UnlockAchivemRequest> unlockValidator,
			IGenericRepository<UserAchievement> userAchievementRepository,
			IGenericRepository<Achievement> achievementRepository,
			IHubContext<NotificationHub> hubContext)
		{
			_userService = userService;
			_mapper = mapper;
			_unlockValidator = unlockValidator;
			_userAchievementRepository = userAchievementRepository;
			_achievementRepository = achievementRepository;
			_hubContext = hubContext;
		}

		public async Task CheckAndUnlockAsync(User user)
		{
			var allAchievements = await _achievementRepository.GetAllAsync(a => !a.IsDeleted);

			foreach (var achievement in allAchievements)
			{
				bool alreadyUnlocked = user.UserAchievements.Any(ua => ua.AchievementId == achievement.Id);
				if (alreadyUnlocked) continue;

				if (CheckCondition(user, achievement))
				{
					await UnclockAchievement(new UnlockAchivemRequest
					{
						UserId = user.Id,
						AchievementId = achievement.Id
					});
				}
			}
		}

		public async Task UnclockAchievement(UnlockAchivemRequest request)
		{
			// validate request
			var validationResult = _unlockValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				// invalid request — nothing to do
				return;
			}

			// ensure user exists
			var userExists = await _userService.IsUserExistAsync(request.UserId);
			if (!userExists)
			{
				return;
			}

			// ensure achievement exists
			var achievement = await _achievementRepository.GetByConditionAsync(a => a.Id == request.AchievementId);
			if (achievement == null || achievement.IsDeleted)
			{
				return;
			}

			// avoid duplicate unlocks
			var existing = await _userAchievementRepository.GetByConditionAsync(ua => ua.UserId == request.UserId && ua.AchievementId == request.AchievementId);
			if (existing != null)
			{
				return;
			}

			// create and persist user achievement
			var userAchievement = _mapper.Map<UserAchievement>(request);

			await _userAchievementRepository.AddAsync(userAchievement);
			var saved = await _userAchievementRepository.SaveChangesAsync();

			if (!saved)
			{
				return;
			}

			// build DTO for notification and send to connected user(s)
			var getAchievemRequest = _mapper.Map<GetAchievemResponse>(achievement);

			await _hubContext.Clients.User(request.UserId.ToString())
				.SendAsync("AchievementUnlocked", getAchievemRequest);

			//Console.WriteLine($"[SignalR] Sent AchievementUnlocked to user {request.UserId}: {JsonSerializer.Serialize(getAchievemRequest)}");
		}

		private bool CheckCondition(User user, Achievement achievement)
		{
			if (user == null || achievement == null)
				return false;

			var required = achievement.ConditionValue;

			switch (achievement.ConditionType)
			{
				case ConditionType.Streak:
					return (user.Streaks ?? Enumerable.Empty<Streak>())
						.Any(s => s.Type == StreakType.Login && s.CurrentCount >= required);

				case ConditionType.MetSchedule:
					return (user.Schedules ?? Enumerable.Empty<Schedule>())
						.Count(s => s.IsCheckin) >= required;

				case ConditionType.DocumentUpload:
					return (user.Workspaces ?? Enumerable.Empty<Workspace>())
						.Sum(ws => (ws.Documents).Count) >= required;

				case ConditionType.NoteCreated:
					return (user.Workspaces ?? Enumerable.Empty<Workspace>())
						.Sum(ws => (ws.Notes).Count) >= required;

				case ConditionType.FlashcardReviewed:
					return (user.Workspaces ?? Enumerable.Empty<Workspace>())
						.Sum(ws => (ws.Flashcards ?? Enumerable.Empty<Flashcard>())
							.Count(fc => fc.LastReviewedAt != null)) >= required;

				default:
					return false;
			}
		}
	}
}
