using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface ISubscriptionService
	{
		public Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration);
		public Task Update(Subscription subscription);
		public Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
	}
}
