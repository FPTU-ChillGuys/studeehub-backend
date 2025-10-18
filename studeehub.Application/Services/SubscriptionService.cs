using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly IMapper _mapper;

		public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper)
		{
			_subscriptionRepository = subscriptionRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
			=> await _subscriptionRepository.GetExpiredSubscriptionsAsync();

		public async Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration)
			=> await _subscriptionRepository.GetExpiringSubscriptionsAsync(daysBeforeExpiration);

		public async Task<BaseResponse<List<GetSubscriptionResponse>>> GetSubscriptionsByUserIdAsync(Guid userId)
		{
			var subscriptions = await _subscriptionRepository.GetAllAsync(
				s => s.UserId == userId,
				include: s => s.Include(s => s.SubscriptionPlan),
				asNoTracking: true);

			// Return empty list (OK) when user has no subscriptions — preferred for list endpoints
			if (subscriptions == null || !subscriptions.Any())
			{
				return BaseResponse<List<GetSubscriptionResponse>>.Ok(new List<GetSubscriptionResponse>());
			}

			var response = _mapper.Map<List<GetSubscriptionResponse>>(subscriptions);
			return BaseResponse<List<GetSubscriptionResponse>>.Ok(response);
		}

		public async Task Update(Subscription subscription)
		{
			_subscriptionRepository.Update(subscription);
			await _subscriptionRepository.SaveChangesAsync();
		}
	}
}
