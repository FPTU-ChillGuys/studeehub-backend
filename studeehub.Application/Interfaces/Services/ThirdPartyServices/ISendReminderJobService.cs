namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISendReminderJobService
	{
		public Task SendStreakRemindersAsync();
		public void ScheduleDailyStreakReminderJob();
		public Task SendScheduleReminderAsync();
		public void ScheduleScheduleReminderJob();
		public Task SendExpiredSubscriptionReminderAsync();
		public Task SendUpcomingSubscriptionReminderAsync();
		public void ScheduleSubscriptionReminderJobs();

	}
}
