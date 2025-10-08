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
				await _scheduleService.UpdateAsync(schedule);
			}

			// Check-in reminders (event already started but not checked in)
			var checkinSchedules = await _scheduleService.GetCheckinSchedulesToRemindAsync(now);

			foreach (var schedule in checkinSchedules)
			{
				var user = schedule.User;
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

		public async Task SendExpiredSubscriptionReminderAsync()
		{
			const int daysBeforeExpiration = 3; // consider moving to configuration
			var now = DateTime.UtcNow;

			var expiringSubscriptions = await _subscriptionService.GetExpiringSubscriptionsAsync(daysBeforeExpiration);
			if (expiringSubscriptions == null || !expiringSubscriptions.Any())
			{
				_logger.LogInformation("No expiring subscriptions found for reminder.");
				return;
			}

			// De-duplicate so each user receives a single reminder (choose the most relevant subscription)
			var latestPerUser = expiringSubscriptions
				.GroupBy(s => s.UserId)
				.Select(g => g.OrderByDescending(s => s.EndDate).First())
				.ToList();

			foreach (var subscription in latestPerUser)
			{
				var user = subscription.User;
				if (user == null || string.IsNullOrWhiteSpace(user.Email))
				{
					_logger.LogWarning("Skipping subscription {SubscriptionId} because user or email is missing", subscription.Id);
					continue;
				}

				var daysLeftDouble = (subscription.EndDate - now).TotalDays;
				var daysLeft = daysLeftDouble <= 0 ? 0 : (int)Math.Ceiling(daysLeftDouble);

				var subject = $"🔔 Your subscription expires in {daysLeft} day{(daysLeft == 1 ? "" : "s")}";
				var body = _emailTemplateService.ExpiredSubscriptionTemplate(
					string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "User" : user.FullName,
					subscription.SubscriptionPlan?.Name ?? "your plan",
					subscription.EndDate);

				try
				{
					await _emailService.SendEmailAsync(user.Email!, subject, body);
					_logger.LogInformation("Sent subscription expiry email to {Email} for subscription {SubscriptionId}", user.Email, subscription.Id);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send subscription expiry email to {Email} for subscription {SubscriptionId}", user.Email, subscription.Id);
				}

				// SignalR push - best effort
				try
				{
					await _hubContext.Clients.User(user.Id.ToString())
						.SendAsync("SubscriptionExpiring", new
						{
							SubscriptionId = subscription.Id,
							ExpiresAt = subscription.EndDate,
							DaysLeft = daysLeft
						});
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "SignalR push failed for subscription reminder to user {UserId}", user.Id);
				}

				subscription.IsExpiryNotified = true;
				subscription.UpdatedAt = DateTime.UtcNow;
				await _subscriptionService.Update(subscription);
			}
		}

		public void ScheduleExpiredSubscriptionReminderJob()
		{
			_recurringJobManager.AddOrUpdate<ISendReminderJobService>(
				"expired-subscription-reminder-job",
				svc => svc.SendExpiredSubscriptionReminderAsync(),
				"0 9 * * *"); // Runs daily at 09:00 UTC
		}
	}
}
