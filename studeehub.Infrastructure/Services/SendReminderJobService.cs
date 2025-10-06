using Hangfire;
using Microsoft.AspNetCore.SignalR;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Infrastructure.Extensions;

namespace studeehub.Infrastructure.Services
{
	public class SendReminderJobService : ISendReminderJobService
	{
		private readonly IEmailService _emailService;
		private readonly IStreakService _streakService;
		private readonly IScheduleService _scheduleService;
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly IRecurringJobManager _recurringJobManager;

		public SendReminderJobService(IEmailService emailService, IStreakService streakService, IEmailTemplateService emailTemplateService, IRecurringJobManager recurringJobManager, IScheduleService scheduleService, IHubContext<NotificationHub> hubContext)
		{
			_emailService = emailService;
			_streakService = streakService;
			_emailTemplateService = emailTemplateService;
			_recurringJobManager = recurringJobManager;
			_scheduleService = scheduleService;
			_hubContext = hubContext;
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

		public async Task SendScheduleReminderAsync()
		{
			var now = DateTime.UtcNow;

			//Pre-event reminders
			var upcomingSchedules = await _scheduleService.GetUpcomingSchedulesToRemindAsync(now);

			foreach (var schedule in upcomingSchedules)
			{
				var user = schedule.User;
				var subject = $"⏰ Reminder: {schedule.Title} starts soon!";
				var body = _emailTemplateService.ScheduleReminderTemplate(user.FullName, schedule.Title, schedule.StartTime);

				// Send Email
				await _emailService.SendEmailAsync(user.Email!, subject, body);

				// Send SignalR
				await _hubContext.Clients.User(user.Id.ToString())
					.SendAsync("ScheduleReminder", new
					{
						ScheduleId = schedule.Id,
						Title = schedule.Title,
						StartTime = schedule.StartTime,
						Description = schedule.Description
					});

				schedule.IsReminded = true;
				await _scheduleService.UpdateAsync(schedule);
			}

			//Check-in reminders (event already started but not checked in)
			var checkinSchedules = await _scheduleService.GetCheckinSchedulesToRemindAsync(now);

			foreach (var schedule in checkinSchedules)
			{
				var user = schedule.User;
				var subject = $"📅 Time to check in: {schedule.Title}";
				var body = _emailTemplateService.ScheduleCheckinTemplate(user.FullName, schedule.Title);

				await _emailService.SendEmailAsync(user.Email!, subject, body);

				await _hubContext.Clients.User(user.Id.ToString())
					.SendAsync("ScheduleCheckinReminder", new
					{
						ScheduleId = schedule.Id,
						Title = schedule.Title,
						StartTime = schedule.StartTime
					});
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

		public void ScheduleScheduleReminderJob()
		{
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"schedule-reminder-job",
				svc => svc.SendScheduleReminderAsync(),
				"*/5 * * * *"); // every 5 minutes
		}
	}
}
