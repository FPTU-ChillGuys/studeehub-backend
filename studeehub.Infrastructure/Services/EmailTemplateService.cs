using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.Infrastructure.Services
{
	public class EmailTemplateService : IEmailTemplateService
	{
		public string GetRegisterTemplate(string username, string verifyUrl)
		{
			return $@"
			<html>
				<body style='font-family: Arial, sans-serif;'>
					<p>Dear {username},</p>
					<p>Thank you for registering with us! Please click <a href='{verifyUrl}'>here</a> to verify your email address.</p>
					<p>If you did not register, please ignore this email.</p>
					<p>Best regards,<br/>EduConnect Team</p>
				</body>
			</html>";
		}

		public string GetForgotPasswordTemplate(string username, string resetUrl)
		{
			return $@"
			<html>
				<body style='font-family: Arial, sans-serif;'>
					<p>Hi {username},</p>
					<p>We received a request to reset your password.</p>
					<p>Click <a href='{resetUrl}'>here</a> to reset your password.</p>
					<p>If you did not request a password reset, please ignore this email. This link will expire soon for security reasons.</p>
					<p>Best regards,<br/>EduConnect Team</p>
				</body>
			</html>";
		}
	}
}
