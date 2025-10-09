namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IEmailTemplateService
	{
		public string ScheduleCheckinTemplate(string fullName, string title);
		public string ScheduleReminderTemplate(string fullName, string title, DateTime startTime);
		public string GetRegisterTemplate(string username, string verifyUrl);
		public string GetForgotPasswordTemplate(string username, string resetUrl);
		public string StreakReminderTemplate(string fullname);
		public string ExpiredSubscriptionTemplate(string fullname, string planName, DateTime endDate);
		public string UpcomingExpiryTemplate(string fullname, string planName, DateTime endDate);

	}
}
