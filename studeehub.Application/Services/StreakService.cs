using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Streak;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Streaks;

namespace studeehub.Application.Services
{
	public class StreakService : IStreakService
	{
		private readonly IMapper _mapper;
		private readonly IValidator<CreateStreakRequest> _createStreakValidator;
		private readonly IValidator<UpdateStreakRequest> _updateStreakValidator;
		private readonly IUserService _userService;
		private readonly IStreakRepository _streakRepository;
		private readonly IUserAchievementService _userAchievementService;

		public StreakService(IMapper mapper, IValidator<CreateStreakRequest> createStreakValidator, IStreakRepository streakRepository, IValidator<UpdateStreakRequest> updateStreakValidator, IUserService userService, IUserAchievementService userAchievementService)
		{
			_mapper = mapper;
			_createStreakValidator = createStreakValidator;
			_streakRepository = streakRepository;
			_updateStreakValidator = updateStreakValidator;
			_userService = userService;
			_userAchievementService = userAchievementService;
		}

		public async Task<BaseResponse<string>> CreateStreakAsync(CreateStreakRequest request)
		{
			var validationResult = _createStreakValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var userExists = await _userService.IsUserExistAsync(request.UserId);
			if (!userExists)
			{
				return BaseResponse<string>.Fail("User not found", ErrorType.NotFound);
			}

			// Prevent duplicate streak for same user and type
			var existing = await _streakRepository.GetByConditionAsync(s => s.UserId == request.UserId && s.Type == request.Type);

			if (existing != null)
				return BaseResponse<string>.Fail("Streak already exists for this type", ErrorType.Conflict);

			var streak = _mapper.Map<Streak>(request);
			await _streakRepository.AddAsync(streak);
			var result = await _streakRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok(streak.Id.ToString(), "Streak created successfully")
				: BaseResponse<string>.Fail("Failed to create streak", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> UpdateStreakAsync(Guid userId, UpdateStreakRequest request)
		{
			var validationResult = _updateStreakValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var existingStreak = await _streakRepository.GetByConditionAsync(s => s.UserId == userId && s.Type == request.Type && s.IsActive);
			if (existingStreak == null)
			{
				var createReq = new CreateStreakRequest
				{
					UserId = userId,
					Type = request.Type
				};

				return await CreateStreakAsync(createReq);
			}

			// Update logic
			var message = string.Empty;
			var today = DateTime.UtcNow.Date;
			var daysDiff = (today - existingStreak.LastUpdated.Date).Days;
			if (daysDiff == 0)
			{
				return BaseResponse<string>.Ok(existingStreak.Id.ToString(), "Streak already updated today");
			}
			else if (daysDiff == 1)
			{
				existingStreak.CurrentCount++;
				if (existingStreak.CurrentCount > existingStreak.LongestCount)
					existingStreak.LongestCount = existingStreak.CurrentCount;

				var currentUser = await _userService.GetUserByIdAsync(userId);
				if (currentUser != null)
				{
					await _userAchievementService.CheckAndUnlockAsync(currentUser);
				}
				message = "Streak incremented";
			}
			else
			{
				existingStreak.CurrentCount = 1;
				message = "Streak reset due to inactivity";
			}

			var updatedStreak = _mapper.Map(request, existingStreak);
			_streakRepository.Update(updatedStreak);
			var result = await _streakRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok(updatedStreak.Id.ToString(), message)
				: BaseResponse<string>.Fail("Failed to update streak", ErrorType.ServerError);
		}

		public async Task<IEnumerable<User>> GetUsersToRemindAsync() => await _streakRepository.GetUsersToRemindAsync();

		public async Task<BaseResponse<List<GetStreakResponse>>> GetStreakByUserIdAsync(Guid userId, StreakType? type)
		{
			if (type.HasValue)
			{
				var streak = await _streakRepository.GetByConditionAsync(s => s.UserId == userId && s.Type == type && s.IsActive);
				if (streak == null)
				{
					return BaseResponse<List<GetStreakResponse>>.Fail("No streak found for this user and type", ErrorType.NotFound);
				}
				else
				{
					var response = _mapper.Map<List<GetStreakResponse>>(new List<Streak> { streak });
					return BaseResponse<List<GetStreakResponse>>.Ok(response);
				}
			}
			else
			{
				var streaks = await _streakRepository.GetAllAsync(s => s.UserId == userId && s.IsActive);
				if (streaks == null || !streaks.Any())
				{
					return BaseResponse<List<GetStreakResponse>>.Fail("No streaks found for this user", ErrorType.NotFound);

				}
				else
				{
					var response = _mapper.Map<List<GetStreakResponse>>(streaks);
					return BaseResponse<List<GetStreakResponse>>.Ok(response);
				}
			}
		}

		public async Task<BaseResponse<GetStreakResponse>> GetStreakByIdAsync(Guid id)
		{
			var streak = await _streakRepository.GetByConditionAsync(s => s.Id == id && s.IsActive);
			if (streak == null)
			{
				return BaseResponse<GetStreakResponse>.Fail("Streak not found", ErrorType.NotFound);

			}
			else
			{
				var response = _mapper.Map<GetStreakResponse>(streak);
				return BaseResponse<GetStreakResponse>.Ok(response);
			}
		}
	}
}
