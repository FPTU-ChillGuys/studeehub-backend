using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class StreakService : IStreakService
	{
		private readonly IMapper _mapper;
		private readonly IValidator<CreateStreakRequest> _createStreakValidator;
		private readonly IValidator<UpdateStreakRequest> _updateStreakValidator;
		private readonly IUserService _userService;
		private readonly IStreakRepository _streakRepository;

		public StreakService(IMapper mapper, IValidator<CreateStreakRequest> createStreakValidator, IStreakRepository streakRepository, IValidator<UpdateStreakRequest> updateStreakValidator, IUserService userService)
		{
			_mapper = mapper;
			_createStreakValidator = createStreakValidator;
			_streakRepository = streakRepository;
			_updateStreakValidator = updateStreakValidator;
			_userService = userService;
		}

		public async Task<BaseResponse<string>> CreateStreakAsync(CreateStreakRequest request)
		{
			var validationResult = _createStreakValidator.Validate(request);
			var userExists = await _userService.IsUserExistAsync(request.UserId);
			if (!validationResult.IsValid || !userExists)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			// Prevent duplicate streak for same user and type
			var existing = await _streakRepository.GetByIdAsync(s => s.UserId == request.UserId && s.Type == request.Type);

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

			var existingStreak = await _streakRepository.GetByIdAsync(s => s.UserId == userId && s.Type == request.Type);
			if (existingStreak == null)
			{
				var createReq = new CreateStreakRequest
				{
					UserId = userId,
					Type = request.Type,
				};

				return await CreateStreakAsync(createReq);
			}

			// Update logic
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
			}
			else
			{
				existingStreak.CurrentCount = 1;
			}

			var updatedStreak = _mapper.Map(request, existingStreak);
			_streakRepository.Update(updatedStreak);
			var result = await _streakRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok(updatedStreak.Id.ToString(), "Streak reset successfully")
				: BaseResponse<string>.Fail("Failed to reset streak", ErrorType.ServerError);
		}

		public Task<IEnumerable<User>> GetUsersToRemindAsync() => _streakRepository.GetUsersToRemindAsync();
	}
}
