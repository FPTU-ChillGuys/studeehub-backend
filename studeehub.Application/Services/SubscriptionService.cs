using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Subscriptions;
using System.Linq.Expressions;

namespace studeehub.Application.Services
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository;
		private readonly IValidator<CreateSubscriptionRequest> _createSubscriptionValidator;
		private readonly IMapper _mapper;

		public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper, IValidator<CreateSubscriptionRequest> createSubscriptionValidator, IGenericRepository<SubscriptionPlan> subscriptionPlanRepository)
		{
			_subscriptionRepository = subscriptionRepository;
			_mapper = mapper;
			_createSubscriptionValidator = createSubscriptionValidator;
			_subscriptionPlanRepository = subscriptionPlanRepository;
		}

		public async Task<BaseResponse<string>> CreateSubscriptionAsync(CreateSubscriptionRequest request)
		{
			var validationResult = await _createSubscriptionValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed.", ErrorType.Validation, errors);
			}

			// Prevent creating a new active subscription when user already has an active one
			var existingActive = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(request.UserId);
			if (existingActive != null && existingActive.Status == SubscriptionStatus.Active && request.Status == SubscriptionStatus.Active)
			{
				return BaseResponse<string>.Fail("User already has an active subscription.", ErrorType.Conflict);
			}

			// Load subscription plan to compute end date if needed
			var plan = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == request.SubscriptionPlanId);
			if (plan == null)
			{
				return BaseResponse<string>.Fail("Subscription plan not found", ErrorType.NotFound);
			}

			var now = DateTime.UtcNow;

			// Build subscription entity explicitly (request has fewer properties now)
			var subscription = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = request.UserId,
				SubscriptionPlanId = request.SubscriptionPlanId,
				Status = request.Status,
				CreatedAt = now,
				UpdatedAt = now,
				IsPreExpiryNotified = false,
				IsPostExpiryNotified = false
			};

			// If subscription should be active or trial, set start/end dates based on plan duration
			if (request.Status == SubscriptionStatus.Active || request.Status == SubscriptionStatus.Trial)
			{
				subscription.StartDate = now;
				subscription.EndDate = plan.DurationInDays > 0 ? now.AddDays(plan.DurationInDays) : now;
			}

			await _subscriptionRepository.AddAsync(subscription);
			var saved = await _subscriptionRepository.SaveChangesAsync();

			return saved
				? BaseResponse<string>.Ok(subscription.Id.ToString(), "Subscription created successfully")
				: BaseResponse<string>.Fail("Failed to create subscription", ErrorType.ServerError);
		}

		public async Task<BaseResponse<GetUserSubscriptionResponse>> GetActiveSubscriptionByUserIdAsync(Guid userId)
		{
			var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
			if (subscription == null)
			{
				return BaseResponse<GetUserSubscriptionResponse>.Fail("No active subscription found for the user.", ErrorType.NotFound);
			}
			var response = _mapper.Map<GetUserSubscriptionResponse>(subscription);
			return BaseResponse<GetUserSubscriptionResponse>.Ok(response);
		}

		public async Task<PagedResponse<GetSubscriptionResponse>> GetAllSubscriptionsAsync(GetPagedAndSortedSubscriptionsRequest request)
		{
			// Start with no-op filter so we can safely compose later
			Expression<Func<Subscription, bool>> filter = s => true;

			// Apply optional filters
			if (request.UserId.HasValue)
				filter = filter.AndAlso(s => s.UserId == request.UserId.Value);

			if (request.SubscriptionPlanId.HasValue)
				filter = filter.AndAlso(s => s.SubscriptionPlanId == request.SubscriptionPlanId.Value);

			if (request.Status.HasValue)
				filter = filter.AndAlso(s => s.Status == request.Status.Value);

			if (request.StartDateFrom.HasValue)
			{
				var dt = request.StartDateFrom.Value.ToDateTime(new TimeOnly(0, 0));
				filter = filter.AndAlso(s => s.StartDate >= dt);
			}

			if (request.StartDateTo.HasValue)
			{
				var dt = request.StartDateTo.Value.ToDateTime(TimeOnly.MaxValue);
				filter = filter.AndAlso(s => s.StartDate <= dt);
			}

			if (request.EndDateFrom.HasValue)
			{
				var dt = request.EndDateFrom.Value.ToDateTime(new TimeOnly(0, 0));
				filter = filter.AndAlso(s => s.EndDate >= dt);
			}

			if (request.EndDateTo.HasValue)
			{
				var dt = request.EndDateTo.Value.ToDateTime(TimeOnly.MaxValue);
				filter = filter.AndAlso(s => s.EndDate <= dt);
			}

			if (!string.IsNullOrWhiteSpace(request.SearchTerm))
			{
				var keyword = request.SearchTerm.Trim();
				// search in user full name or email or subscription plan name
				filter = filter.AndAlso(s =>
					(s.User != null && (s.User.FullName != null && s.User.FullName.Contains(keyword) || s.User.Email != null && s.User.Email.Contains(keyword)))
					|| (s.SubscriptionPlan != null && s.SubscriptionPlan.Name != null && s.SubscriptionPlan.Name.Contains(keyword))
				);
			}

			var paged = await _subscriptionRepository.GetPagedAsync(
				filter: filter,
				include: q => q.Include(s => s.SubscriptionPlan).Include(s => s.User),
				orderBy: request.SortBy == null ? null : (q => q.ApplySorting(request.SortBy, request.SortDescending)),
				pageNumber: request.PageNumber,
				pageSize: request.PageSize,
				asNoTracking: true);

			var items = paged.Items;
			var totalCount = paged.TotalCount;

			if (items == null || !items.Any())
			{
				return PagedResponse<GetSubscriptionResponse>.Ok(new List<GetSubscriptionResponse>(), 0, request.PageNumber, request.PageSize);
			}

			var responses = _mapper.Map<List<GetSubscriptionResponse>>(items);
			return PagedResponse<GetSubscriptionResponse>.Ok(responses, totalCount, request.PageNumber, request.PageSize);
		}

		public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
			=> await _subscriptionRepository.GetExpiredSubscriptionsAsync();

		public async Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration)
			=> await _subscriptionRepository.GetExpiringSubscriptionsAsync(daysBeforeExpiration);

		public async Task<BaseResponse<List<GetUserSubscriptionResponse>>> GetSubscriptionsByUserIdAsync(Guid userId)
		{
			var subscriptions = await _subscriptionRepository.GetAllAsync(
				s => s.UserId == userId,
				include: s => s.Include(s => s.SubscriptionPlan),
				asNoTracking: true);

			// Return empty list (OK) when user has no subscriptions — preferred for list endpoints
			if (subscriptions == null || !subscriptions.Any())
			{
				return BaseResponse<List<GetUserSubscriptionResponse>>.Ok(new List<GetUserSubscriptionResponse>());
			}

			var response = _mapper.Map<List<GetUserSubscriptionResponse>>(subscriptions);
			return BaseResponse<List<GetUserSubscriptionResponse>>.Ok(response);
		}

		public async Task Update(Subscription subscription)
		{
			_subscriptionRepository.Update(subscription);
			await _subscriptionRepository.SaveChangesAsync();
		}

		public async Task<BaseResponse<string>> UpdateSubscriptionStatusAsync(Guid subscriptionId, SubscriptionStatus newStatus)
		{
			var subscription = await _subscriptionRepository.GetByConditionAsync(
				s => s.Id == subscriptionId,
				include: q => q.Include(s => s.SubscriptionPlan));
			if (subscription == null)
				return BaseResponse<string>.Fail("Subscription not found", ErrorType.NotFound);

			var oldStatus = subscription.Status;

			// No-op if the status is the same
			if (oldStatus == newStatus)
			{
				subscription.UpdatedAt = DateTime.UtcNow;
				_subscriptionRepository.Update(subscription);
				var savedSame = await _subscriptionRepository.SaveChangesAsync();
				return savedSame
					? BaseResponse<string>.Ok("Subscription status unchanged")
					: BaseResponse<string>.Fail("Failed to persist subscription status", ErrorType.ServerError);
			}

			// Helper local funcs to classify statuses
			static bool IsClosedStatus(SubscriptionStatus s)
				=> s == SubscriptionStatus.Expired || s == SubscriptionStatus.Cancelled || s == SubscriptionStatus.Failed;

			static bool IsOpenStatus(SubscriptionStatus s)
				=> s == SubscriptionStatus.Pending || s == SubscriptionStatus.Trial || s == SubscriptionStatus.Active;

			// Case 1: Reactivate (closed -> open)
			if (IsClosedStatus(oldStatus) && IsOpenStatus(newStatus))
			{
				var now = DateTime.UtcNow;
				subscription.StartDate = now;
				// compute end date using loaded SubscriptionPlan when available
				if (subscription.SubscriptionPlan != null)
				{
					subscription.EndDate = now.AddDays(subscription.SubscriptionPlan.DurationInDays);
				}
				else if (subscription.SubscriptionPlanId != Guid.Empty)
				{
					var plan = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == subscription.SubscriptionPlanId);
					if (plan != null)
						subscription.EndDate = now.AddDays(plan.DurationInDays);
				}
			}

			// Case 2: Trial -> Active
			if (oldStatus == SubscriptionStatus.Trial && newStatus == SubscriptionStatus.Active)
			{
				// Ensure StartDate is set
				if (subscription.StartDate == default)
					subscription.StartDate = DateTime.UtcNow;

				if (subscription.SubscriptionPlan != null)
				{
					subscription.EndDate = subscription.StartDate.AddDays(subscription.SubscriptionPlan.DurationInDays);
				}
				else if (subscription.SubscriptionPlanId != Guid.Empty)
				{
					var plan = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == subscription.SubscriptionPlanId);
					if (plan != null)
						subscription.EndDate = subscription.StartDate.AddDays(plan.DurationInDays);
				}
			}

			// Case 3: Active -> Closed (Expired/Cancelled/Failed)
			// Intentionally do not modify StartDate/EndDate

			subscription.Status = newStatus;
			subscription.UpdatedAt = DateTime.UtcNow;

			_subscriptionRepository.Update(subscription);
			var saved = await _subscriptionRepository.SaveChangesAsync();

			return saved
				? BaseResponse<string>.Ok("Subscription status updated successfully")
				: BaseResponse<string>.Fail("Failed to update status", ErrorType.ServerError);
		}

	}
}
