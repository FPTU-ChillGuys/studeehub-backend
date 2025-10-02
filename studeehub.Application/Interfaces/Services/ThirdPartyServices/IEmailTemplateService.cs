namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IEmailTemplateService
	{
		string GetRegisterTemplate(string username, string verifyUrl);
		string GetForgotPasswordTemplate(string username, string resetUrl);
	}
}
