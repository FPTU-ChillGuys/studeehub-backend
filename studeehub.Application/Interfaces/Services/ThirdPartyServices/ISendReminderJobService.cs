namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISendReminderJobService
	{
		public Task SendStreakRemindersAsync();
		public void ScheduleDailyStreakReminderJob();
	}
}
