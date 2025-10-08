using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Subscriptions;

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

			var existingPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Code == request.Code);
			if (existingPlan != null)
			{
				return BaseResponse<string>.Fail($"A subscription plan with code '{request.Code}' already exists.", ErrorType.Conflict);
			}

			var newPlan = _mapper.Map<SubscriptionPlan>(request);
			await _subPlanRepository.AddAsync(newPlan);
			var result = await _subPlanRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok(newPlan.Id.ToString(), "Subscription plan created successfully.")
				: BaseResponse<string>.Fail("Failed to create subscription plan.", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteSubPlanAsync(Guid id)
		{
			var existingPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Id == id);
			if (existingPlan == null)
			{
				return BaseResponse<string>.Fail("Subscription plan not found.", ErrorType.NotFound);
			}

			var activeSubscriptions = existingPlan.Subscriptions.Any(s => s.Status == SubscriptionStatus.Active);
			if (activeSubscriptions)
			{
				return BaseResponse<string>.Fail("Cannot delete subscription plan with active subscriptions.", ErrorType.Conflict);
			}

			_subPlanRepository.Remove(existingPlan);
			var result = await _subPlanRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Subscription plan deleted successfully.")
				: BaseResponse<string>.Fail("Failed to delete subscription plan.", ErrorType.ServerError);
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

			// Check for code uniqueness if the code is being changed
			if (!string.Equals(existingPlan.Code, request.Code, StringComparison.OrdinalIgnoreCase))
			{
				var codeConflictPlan = await _subPlanRepository.GetByConditionAsync(sp => sp.Code == request.Code);
				if (codeConflictPlan != null)
				{
					return BaseResponse<string>.Fail($"A subscription plan with code '{request.Code}' already exists.", ErrorType.Conflict);
				}
			}

			_mapper.Map(request, existingPlan);
			_subPlanRepository.Update(existingPlan);
			var result = await _subPlanRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Subscription plan updated successfully.")
				: BaseResponse<string>.Fail("Failed to update subscription plan.", ErrorType.ServerError);
		}
	}
}
