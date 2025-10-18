using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Achievement;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using System.Linq.Expressions;

namespace studeehub.Application.Services
{
	public class AchievementService : IAchievementService
	{
		private readonly IGenericRepository<Achievement> _achievementRepository;
		private readonly IGenericRepository<UserAchievement> _userAchievementRepository;
		private readonly IValidator<CreateAchievemRequest> _createValidator;
		private readonly IValidator<UpdateAchievemRequest> _updateValidator;
		private readonly IValidator<GetAchievemsRequest> _getValidator;
		private readonly IMapper _mapper;

		public AchievementService(IGenericRepository<Achievement> achievementRepository, IMapper mapper, IValidator<CreateAchievemRequest> createValidator, IValidator<UpdateAchievemRequest> updateValidator, IGenericRepository<UserAchievement> userAchievementRepository, IValidator<GetAchievemsRequest> getValidator)
		{
			_achievementRepository = achievementRepository;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
			_userAchievementRepository = userAchievementRepository;
			_getValidator = getValidator;
		}

		public async Task<BaseResponse<string>> CreateAchievementAsync(CreateAchievemRequest request)
		{
			var validationResult = _createValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var existingAchievement = await _achievementRepository.GetByConditionAsync(a => a.Code == request.Code);
			if (existingAchievement != null)
			{
				return BaseResponse<string>.Fail($"Achievement with code {request.Code} already exists", ErrorType.Conflict);
			}

			var achievement = _mapper.Map<Achievement>(request);
			await _achievementRepository.AddAsync(achievement);
			var result = await _achievementRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Achievement created successfully")
				: BaseResponse<string>.Fail("Failed to create achievement", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteAchievementAsync(Guid id)
		{
			var achievement = await _achievementRepository.GetByConditionAsync(a => a.Id == id && !a.IsDeleted);
			if (achievement == null)
				return BaseResponse<string>.Fail("Achievement not found", ErrorType.NotFound);

			var isAssigned = await _userAchievementRepository.AnyAsync(ua => ua.AchievementId == id);
			if (isAssigned)
			{
				achievement.IsDeleted = true;
				achievement.DeletedAt = DateTime.UtcNow;
				_achievementRepository.Update(achievement);
			}
			else
			{
				_achievementRepository.Remove(achievement);
			}

			var result = await _achievementRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Achievement deleted successfully")
				: BaseResponse<string>.Fail("Failed to delete achievement", ErrorType.ServerError);
		}

		public async Task<BaseResponse<GetAchievemResponse>> GetAchievementByIdAsync(Guid id)
		{
			var achievement = await _achievementRepository.GetByConditionAsync(a => a.Id == id && !a.IsDeleted, asNoTracking: true);
			if (achievement == null)
			{
				return BaseResponse<GetAchievemResponse>.Fail("Achievement not found", ErrorType.NotFound);
			}
			var response = _mapper.Map<GetAchievemResponse>(achievement);
			return BaseResponse<GetAchievemResponse>.Ok(response);
		}

		public async Task<PagedResponse<GetAchievemResponse>> GetPagedAchievementsAsync(GetAchievemsRequest request)
		{
			var validationResult = await _getValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return PagedResponse<GetAchievemResponse>.Fail(
																	"Invalid request",
																	ErrorType.Validation,
																	errors,
																	request.PageNumber,
																	request.PageSize);
			}

			Expression<Func<Achievement, bool>> filter = a => !a.IsDeleted;
			if (!string.IsNullOrWhiteSpace(request.SearchTerm))
			{
				filter = filter.AndAlso(a =>
											a.Name.Contains(request.SearchTerm) ||
											a.Description != null && a.Description.Contains(request.SearchTerm));
			}

			if (request.UserId.HasValue)
			{
				filter = filter.AndAlso(a => a.UserAchievements.Any(ua => ua.UserId == request.UserId.Value));
			}

			if (request.ConditionType.HasValue)
			{
				filter = filter.AndAlso(a => a.ConditionType == request.ConditionType.Value);
			}

			if (request.RewardType.HasValue)
			{
				filter = filter.AndAlso(a => a.RewardType == request.RewardType.Value);
			}

			var (achievements, totalCount) = await _achievementRepository.GetPagedAsync(
				filter: filter,
				orderBy: a => a.ApplySorting(request.SortBy, request.SortDescending),
				include: null,
				pageNumber: request.PageNumber,
				pageSize: request.PageSize,
				asNoTracking: true);

			if (!achievements.Any())
			{
				return PagedResponse<GetAchievemResponse>.Fail("No Achievement matching", ErrorType.NotFound, null, request.PageNumber, request.PageSize);
			}

			var responses = _mapper.Map<List<GetAchievemResponse>>(achievements);
			return PagedResponse<GetAchievemResponse>.Ok(responses, totalCount, page: request.PageNumber, pageSize: request.PageSize);
		}

		public async Task<BaseResponse<string>> UpdateAchievementAsync(Guid id, UpdateAchievemRequest request)
		{
			var validationResult = _updateValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var achievement = await _achievementRepository.GetByConditionAsync(a => a.Id == id);

			if (achievement == null)
			{
				return BaseResponse<string>.Fail("Achievement not found", ErrorType.NotFound);
			}

			var isExisting = await _achievementRepository.AnyAsync(a => a.Code == request.Code && a.Id != id);
			if (isExisting)
			{
				return BaseResponse<string>.Fail($"Achievement with code {request.Code} already exists", ErrorType.Conflict);
			}

			var updated = _mapper.Map(request, achievement);
			updated.UpdatedAt = DateTime.UtcNow;

			_achievementRepository.Update(updated);
			var result = await _achievementRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Achievement updated successfully")
				: BaseResponse<string>.Fail("Failed to update achievement", ErrorType.ServerError);
		}

	}
}
