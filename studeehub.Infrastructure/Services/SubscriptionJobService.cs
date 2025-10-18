using Microsoft.Extensions.Logging;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Infrastructure.Services
{
	public class SubscriptionJobService : ISubscriptionJobService
	{
		private readonly IGenericRepository<Subscription> _subscriptionRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<SubscriptionJobService> _logger;

		public SubscriptionJobService(
			IUnitOfWork unitOfWork,
			ILogger<SubscriptionJobService> logger,
			IGenericRepository<Subscription> subscriptionRepository)
		{
			_subscriptionRepository = subscriptionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		/// <summary>
		/// Check subscriptions that have been pending too long and mark them as Failed.
		/// </summary>
		public async Task CheckPendingSubscriptionsAsync()
		{
			var threshold = DateTime.UtcNow.AddMinutes(-30);

			var staleSubs = await _subscriptionRepository.GetAllAsync(
				s => s.Status == SubscriptionStatus.Pending && s.CreatedAt < threshold
			);

			if (staleSubs.Any())
			{
				foreach (var sub in staleSubs)
					sub.Status = SubscriptionStatus.Failed;

				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("[SubscriptionJob] Marked {Count} stale subscriptions as Failed.", staleSubs.Count());
			}
			else
			{
				_logger.LogInformation("[SubscriptionJob] No pending subscriptions to update.");
			}
		}

		/// <summary>
		/// Check subscriptions that have reached their end date and mark them as Expired.
		/// </summary>
		public async Task CheckExpiredSubscriptionsAsync()
		{
			var now = DateTime.UtcNow;

			var expiredSubs = await _subscriptionRepository.GetAllAsync(
				s => (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial)
					 && s.EndDate <= now
			);

			if (expiredSubs.Any())
			{
				foreach (var sub in expiredSubs)
				{
					sub.Status = SubscriptionStatus.Expired;
					sub.UpdatedAt = now;
				}

				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("[SubscriptionJob] Marked {Count} subscriptions as Expired.", expiredSubs.Count());
			}
			else
			{
				_logger.LogInformation("[SubscriptionJob] No expired subscriptions found.");
			}
		}
	}
}
