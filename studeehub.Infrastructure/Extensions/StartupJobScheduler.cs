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

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
