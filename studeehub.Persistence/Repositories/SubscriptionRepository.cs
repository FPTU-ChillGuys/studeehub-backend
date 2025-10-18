using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
	{
		public SubscriptionRepository(StudeeHubDBContext context) : base(context)
		{
		}

		public async Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration)
		{
			var now = DateTime.UtcNow;
			var endWindow = now.AddDays(daysBeforeExpiration);

			// include User and SubscriptionPlan to avoid N+1 when sending emails
			return await _context.Subscriptions
				.AsNoTracking()
				.Include(s => s.User)
				.Include(s => s.SubscriptionPlan)
				.Where(s =>
					(
						s.Status == SubscriptionStatus.Trial ||
						s.Status == SubscriptionStatus.Active
					) &&
					s.EndDate >= now &&                      // not already expired
					s.EndDate <= endWindow                  // within the window
				)
				.ToListAsync();
		}

		public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
		{
			var now = DateTime.UtcNow;
			return await _context.Subscriptions
				.AsNoTracking()
				.Include(s => s.User)
				.Include(s => s.SubscriptionPlan)
				.Where(s =>
					s.EndDate <= now &&
					!s.IsPostExpiryNotified &&
					(
						s.Status == SubscriptionStatus.Active ||
						s.Status == SubscriptionStatus.Trial)
					)
				.ToListAsync();
		}
	}
}
