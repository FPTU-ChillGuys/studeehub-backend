using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.SubPlan;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class SubPlanService : ISubPlanService
	{
		private readonly IGenericRepository<SubscriptionPlan> _subPlanRepository;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateSubPlanRequest> _createValidator;
		private readonly IValidator<UpdateSubPlanRequest> _updateValidator;

		public SubPlanService(IGenericRepository<SubscriptionPlan> subPlanRepository, IMapper mapper, IValidator<CreateSubPlanRequest> createValidator, IValidator<UpdateSubPlanRequest> updateValidator)
		{
			_subPlanRepository = subPlanRepository;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<BaseResponse<string>> CreateSubPlanAsync(CreateSubPlanRequest request)
		{
			var validationResult = await _createValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var existingPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Code == request.Code && !sp.IsDeleted);
			if (existingPlan != null)
			{
				return BaseResponse<string>.Fail($"A subscription plan with code '{request.Code}' already exists.", ErrorType.Conflict);
			}

			var newPlan = _mapper.Map<SubscriptionPlan>(request);

			// Ensure entity has an Id and timestamps before persisting
			newPlan.Id = Guid.NewGuid();
			newPlan.CreatedAt = DateTime.UtcNow;
			newPlan.UpdatedAt = DateTime.UtcNow;

			await _subPlanRepository.AddAsync(newPlan);
			var result = await _subPlanRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok(newPlan.Id.ToString(), "Subscription plan created successfully.")
				: BaseResponse<string>.Fail("Failed to create subscription plan.", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteSubPlanAsync(Guid id)
		{
			var existingPlan = await _subPlanRepository.GetByConditionAsync(
				sp => sp.Id == id && !sp.IsDeleted,
				include: q => q.Include(sp => sp.Subscriptions)
			);

			if (existingPlan == null)
				return BaseResponse<string>.Fail("Subscription plan not found.", ErrorType.NotFound);

			// If any subscriptions exist, even if expired/cancelled → keep for data integrity
			var hasAnySubscriptions = existingPlan.Subscriptions?.Any() ?? false;

			if (hasAnySubscriptions)
			{
				existingPlan.IsDeleted = true;
				existingPlan.DeletedAt = DateTime.UtcNow;
				_subPlanRepository.Update(existingPlan);
			}
			else
			{
				_subPlanRepository.Remove(existingPlan);
			}

			var result = await _subPlanRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Subscription plan deleted successfully.")
				: BaseResponse<string>.Fail("Failed to delete subscription plan.", ErrorType.ServerError);
		}

		public async Task<BaseResponse<List<GetSubPlanResponse>>> GetAllSubPlansAsync()
		{
			var plans = await _subPlanRepository.GetAllAsync(sp => !sp.IsDeleted);
			if (plans == null || !plans.Any())
			{
				return BaseResponse<List<GetSubPlanResponse>>.Fail("No subscription plans found.", ErrorType.NotFound);
			}
			var response = _mapper.Map<List<GetSubPlanResponse>>(plans);
			return BaseResponse<List<GetSubPlanResponse>>.Ok(response);
		}

		public async Task<BaseResponse<GetSubPlanResponse>> GetSubPlanByIdAsync(Guid id)
		{
			var existingPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Id == id && !sp.IsDeleted);
			if (existingPlan == null)
			{
				return BaseResponse<GetSubPlanResponse>.Fail("Subscription plan not found.", ErrorType.NotFound);
			}
			var response = _mapper.Map<GetSubPlanResponse>(existingPlan);
			return BaseResponse<GetSubPlanResponse>.Ok(response);
		}

		public async Task<BaseResponse<string>> UpdateSubPlanAsync(Guid id, UpdateSubPlanRequest request)
		{
			var validationResult = await _updateValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var existingPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Id == id);
			if (existingPlan == null)
			{
				return BaseResponse<string>.Fail("Subscription plan not found.", ErrorType.NotFound);
			}

			// Check for code uniqueness if the code is being changed (ignore deleted plans)
			if (!string.Equals(existingPlan.Code, request.Code, StringComparison.OrdinalIgnoreCase))
			{
				var codeConflictPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Code == request.Code && !sp.IsDeleted);
				if (codeConflictPlan != null)
				{
					return BaseResponse<string>.Fail($"A subscription plan with code '{request.Code}' already exists.", ErrorType.Conflict);
				}
			}

			// Map updates (SubscriptionPlanRegister now maps IsActive)
			_mapper.Map(request, existingPlan);
			existingPlan.UpdatedAt = DateTime.UtcNow;

			_subPlanRepository.Update(existingPlan);
			var result = await _subPlanRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Subscription plan updated successfully.")
				: BaseResponse<string>.Fail("Failed to update subscription plan.", ErrorType.ServerError);
		}
	}
}
