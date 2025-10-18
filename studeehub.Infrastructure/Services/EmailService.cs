using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.Infrastructure.Services
{
	public class EmailService(IConfiguration configuration) : IEmailService
	{
		public async Task SendEmailAsync(string to, string subject, string htmlBody)
		{
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress("StudeeHub", configuration["EmailAccount:SmtpUser"]));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;
			email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(
				configuration["EmailSettings:SmtpServer"],
				int.Parse(configuration["EmailSettings:SmtpPort"]!),
				MailKit.Security.SecureSocketOptions.StartTls
			);
			await smtp.AuthenticateAsync(
				configuration["EmailAccount:SmtpUser"],
				configuration["EmailAccount:SmtpPass"]
			);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
	}
}
