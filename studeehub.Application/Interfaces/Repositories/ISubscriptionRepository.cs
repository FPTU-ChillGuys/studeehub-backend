using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface ISubscriptionRepository : IGenericRepository<Subscription>
	{
		public Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId);
		public Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration);
		public Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();

	}
}
