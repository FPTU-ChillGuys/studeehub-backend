using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
		private readonly ISubscriptionService _subscriptionService;
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly ILogger<SendReminderJobService> _logger;

		public SendReminderJobService(
			IEmailService emailService,
			IStreakService streakService,
			IEmailTemplateService emailTemplateService,
			IRecurringJobManager recurringJobManager,
			IScheduleService scheduleService,
			IHubContext<NotificationHub> hubContext,
			ISubscriptionService subscriptionService,
			ILogger<SendReminderJobService> logger)
		{
			_emailService = emailService;
			_streakService = streakService;
			_emailTemplateService = emailTemplateService;
			_recurringJobManager = recurringJobManager;
			_scheduleService = scheduleService;
			_hubContext = hubContext;
			_subscriptionService = subscriptionService;
			_logger = logger;
		}

		public async Task SendStreakRemindersAsync()
		{
			var inactiveUsers = await _streakService.GetUsersToRemindAsync();

			foreach (var user in inactiveUsers)
			{
				try
				{
					var template = _emailTemplateService.StreakReminderTemplate(user.FullName);
					if (string.IsNullOrWhiteSpace(user.Email))
					{
						_logger.LogWarning("Skipping streak reminder because email is missing for user {UserId}", user.Id);
						continue;
					}
					await _emailService.SendEmailAsync(user.Email!, "🔥 Don’t lose your streak!", template);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send streak reminder to {Email}", user.Email);
				}
			}
		}

		public async Task SendScheduleReminderAsync()
		{
			var now = DateTime.UtcNow;

			// Pre-event reminders
			var upcomingSchedules = await _scheduleService.GetUpcomingSchedulesToRemindAsync(now);

			foreach (var schedule in upcomingSchedules)
			{
				var user = schedule.User;
				if (user == null)
				{
					_logger.LogWarning("Skipping schedule reminder for schedule {ScheduleId} because User is null", schedule.Id);
					continue;
				}
				if (string.IsNullOrWhiteSpace(user.Email))
				{
					_logger.LogWarning("Skipping schedule reminder for schedule {ScheduleId} because user {UserId} has no email", schedule.Id, user.Id);
					continue;
				}

				var subject = $"⏰ Reminder: {schedule.Title} starts soon!";
				var body = _emailTemplateService.ScheduleReminderTemplate(user.FullName, schedule.Title, schedule.StartTime);

				try
				{
					await _emailService.SendEmailAsync(user.Email!, subject, body);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send schedule reminder email to {Email} for schedule {ScheduleId}", user.Email, schedule.Id);
				}

				try
				{
					await _hubContext.Clients.User(user.Id.ToString())
						.SendAsync("ScheduleReminder", new
						{
							ScheduleId = schedule.Id,
							schedule.Title,
							schedule.StartTime,
							schedule.Description
						});
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "SignalR ScheduleReminder failed for user {UserId}", user.Id);
				}

				schedule.IsReminded = true;

				try
				{
					await _scheduleService.UpdateAsync(schedule);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to update schedule {ScheduleId} after sending reminder", schedule.Id);
				}
			}

			// Check-in reminders (event already started but not checked in)
			var checkinSchedules = await _scheduleService.GetCheckinSchedulesToRemindAsync(now);

			foreach (var schedule in checkinSchedules)
			{
				var user = schedule.User;
				if (user == null || string.IsNullOrWhiteSpace(user.Email))
				{
					_logger.LogWarning("Skipping check-in reminder for schedule {ScheduleId} because user or email missing", schedule.Id);
					continue;
				}

				var subject = $"📅 Time to check in: {schedule.Title}";
				var body = _emailTemplateService.ScheduleCheckinTemplate(user.FullName, schedule.Title);

				try
				{
					await _emailService.SendEmailAsync(user.Email!, subject, body);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send check-in reminder to {Email} for schedule {ScheduleId}", user.Email, schedule.Id);
				}

				try
				{
					await _hubContext.Clients.User(user.Id.ToString())
						.SendAsync("ScheduleCheckinReminder", new
						{
							ScheduleId = schedule.Id,
							schedule.Title,
							schedule.StartTime
						});
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "SignalR ScheduleCheckinReminder failed for user {UserId}", user.Id);
				}
			}
		}

		public async Task SendUpcomingSubscriptionReminderAsync()
		{
			const int daysBeforeExpiration = 3;
			var now = DateTime.UtcNow;

			var upcoming = await _subscriptionService.GetExpiringSubscriptionsAsync(daysBeforeExpiration);

			if (upcoming == null || upcoming.Count == 0)
			{
				_logger.LogInformation("No upcoming subscriptions found for pre-expiry reminder.");
				return;
			}

			foreach (var sub in upcoming)
			{
				if (sub.IsPreExpiryNotified) continue;
				var user = sub.User;
				if (user == null || string.IsNullOrWhiteSpace(user.Email)) continue;

				var daysLeft = Math.Max(0, (int)Math.Ceiling((sub.EndDate - now).TotalDays));

				var subject = $"⏰ Gói StudeeHub của bạn sẽ hết hạn sau {daysLeft} ngày";
				var body = _emailTemplateService.UpcomingExpiryTemplate(
					string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "Học viên" : user.FullName,
					sub.SubscriptionPlan?.Name ?? "Gói hiện tại",
					sub.EndDate);

				try
				{
					await _emailService.SendEmailAsync(user.Email!, subject, body);
					sub.IsPreExpiryNotified = true;
					sub.PreExpiryNotifiedAt = DateTime.UtcNow;

					await _subscriptionService.Update(sub);
					_logger.LogInformation("Sent pre-expiry email to {Email}", user.Email);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send pre-expiry email to {Email}", user.Email);
				}
			}
		}

		public async Task SendExpiredSubscriptionReminderAsync()
		{
			var now = DateTime.UtcNow;

			var expiredSubs = await _subscriptionService.GetExpiredSubscriptionsAsync();

			if (expiredSubs == null || !expiredSubs.Any())
			{
				_logger.LogInformation("No expired subscriptions found for post-expiry reminder.");
				return;
			}

			foreach (var sub in expiredSubs)
			{
				if (sub.IsPostExpiryNotified) continue;
				if (sub.EndDate > now) continue;

				var user = sub.User;
				if (user == null || string.IsNullOrWhiteSpace(user.Email)) continue;

				var subject = "⚠️ Gói StudeeHub của bạn đã hết hạn";
				var body = _emailTemplateService.ExpiredSubscriptionTemplate(
					string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "Học viên" : user.FullName,
					sub.SubscriptionPlan?.Name ?? "Gói đăng ký",
					sub.EndDate);

				try
				{
					await _emailService.SendEmailAsync(user.Email!, subject, body);
					sub.IsPostExpiryNotified = true;
					sub.PostExpiryNotifiedAt = DateTime.UtcNow;

					await _subscriptionService.Update(sub);
					_logger.LogInformation("Sent post-expiry email to {Email}", user.Email);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send post-expiry email to {Email}", user.Email);
				}
			}
		}

		public void ScheduleDailyStreakReminderJob()
		{
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"daily-streak-reminder-job",
				svc => svc.SendStreakRemindersAsync(),
				"0 20 * * *"); // Runs at 20:00 UTC
		}

		public void ScheduleScheduleReminderJob()
		{
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"schedule-reminder-job",
				svc => svc.SendScheduleReminderAsync(),
				"*/5 * * * *"); // every 5 minutes
		}

		public void ScheduleSubscriptionReminderJobs()
		{
			// Pre-expiry reminder (daily 9 AM UTC)
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"upcoming-subscription-reminder-job",
				svc => svc.SendUpcomingSubscriptionReminderAsync(),
				"0 9 * * *");

			// Post-expiry follow-up (daily 10 AM UTC)
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"expired-subscription-reminder-job",
				svc => svc.SendExpiredSubscriptionReminderAsync(),
				"0 10 * * *");
		}
	}
}
