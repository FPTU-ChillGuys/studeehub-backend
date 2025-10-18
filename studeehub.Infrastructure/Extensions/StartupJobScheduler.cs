using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.Infrastructure.Extensions
{
	public class StartupJobScheduler : IHostedService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public StartupJobScheduler(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var jobService = scope.ServiceProvider.GetRequiredService<ISendReminderJobService>();

			jobService.ScheduleDailyStreakReminderJob();
			jobService.ScheduleScheduleReminderJob();
			jobService.ScheduleSubscriptionReminderJobs();

			var subscriptionJob = scope.ServiceProvider.GetRequiredService<ISubscriptionJobService>();
			RecurringJob.AddOrUpdate<ISubscriptionJobService>(
				"check-pending-subscriptions",
				svc => svc.CheckPendingSubscriptionsAsync(),
				"*/15 * * * *" // every 15 minutes
			);
			RecurringJob.AddOrUpdate<ISubscriptionJobService>(
				"check-expired-subscriptions",
				svc => svc.CheckExpiredSubscriptionsAsync(),
				"0 0 * * *" // daily at midnight
			);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
