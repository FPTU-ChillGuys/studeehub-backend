using Hangfire;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.Infrastructure.Services
{
	public class SendReminderJobService : ISendReminderJobService
	{
		private readonly IEmailService _emailService;
		private readonly IStreakService _streakService;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly IRecurringJobManager _recurringJobManager;

		public SendReminderJobService(IEmailService emailService, IStreakService streakService, IEmailTemplateService emailTemplateService, IRecurringJobManager recurringJobManager)
		{
			_emailService = emailService;
			_streakService = streakService;
			_emailTemplateService = emailTemplateService;
			_recurringJobManager = recurringJobManager;
		}

		public async Task SendStreakRemindersAsync()
		{
			var inactiveUsers = await _streakService.GetUsersToRemindAsync();

			foreach (var user in inactiveUsers)
			{
				var template = _emailTemplateService.StreakReminderTemplate(user.FullName);
				await _emailService.SendEmailAsync(user.Email!, "🔥 Don’t lose your streak!", template);
			}
		}

		public void ScheduleDailyStreakReminderJob()
		{
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"daily-streak-reminder-job",
				svc => svc.SendStreakRemindersAsync(),
                //"* * * * *"); // Run after 1 minute for testing
                "0 20 * * *"); // Runs at 8:00 PM UTC
		}
	}
}
