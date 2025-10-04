namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IEmailTemplateService
	{
		public string GetRegisterTemplate(string username, string verifyUrl);
		public string GetForgotPasswordTemplate(string username, string resetUrl);
		public string StreakReminderTemplate(string fullname);

	}
}
