using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _subscriptionRepository;

		public SubscriptionService(ISubscriptionRepository subscriptionRepository)
		{
			_subscriptionRepository = subscriptionRepository;
		}

		public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
		 => await _subscriptionRepository.GetExpiredSubscriptionsAsync();

		public async Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration)
			=> await _subscriptionRepository.GetExpiringSubscriptionsAsync(daysBeforeExpiration);

		public async Task Update(Subscription subscription)
		{
			_subscriptionRepository.Update(subscription);
			await _subscriptionRepository.SaveChangesAsync();
		}
	}
}
