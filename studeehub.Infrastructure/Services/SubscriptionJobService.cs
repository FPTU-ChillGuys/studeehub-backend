using Microsoft.Extensions.Logging;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			_subscriptionRepository = subscriptionRepository;
		}

		public async Task CheckPendingSubscriptionsAsync()
        {
            var threshold = DateTime.UtcNow.AddMinutes(-30);

            var staleSubs = await _subscriptionRepository.GetAllAsync(
                s => s.Status == SubscriptionStatus.Pending && s.CreateAt < threshold
            );

            if (staleSubs.Any())
            {
                foreach (var sub in staleSubs)
                    sub.Status = SubscriptionStatus.Failed;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"[SubscriptionJob] Marked {staleSubs.Count()} stale subscriptions as Failed.");
            }
            else
            {
                _logger.LogInformation("[SubscriptionJob] No pending subscriptions to update.");
            }
        }
    }
}
