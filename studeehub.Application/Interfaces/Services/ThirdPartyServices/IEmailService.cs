namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IEmailService
	{
		public Task SendEmailAsync(string to, string subject, string htmlBody);
	}
}
