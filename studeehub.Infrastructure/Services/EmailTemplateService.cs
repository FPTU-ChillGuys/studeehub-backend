using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using System.Net;

namespace studeehub.Infrastructure.Services
{
	public class EmailTemplateService : IEmailTemplateService
	{
		private string WrapHtml(string preheader, string title, string bodyHtml)
		{
			return $@"
			<!doctype html>
			<html>
			<head>
				<meta charset='utf-8'/>
				<meta name='viewport' content='width=device-width,initial-scale=1'/>
				<title>{WebUtility.HtmlEncode(title)}</title>
				<style>
					body {{ margin:0; padding:0; font-family: Arial, sans-serif; background:#f4f6f8; color:#333; }}
					.container {{ max-width:600px; margin:24px auto; background:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 4px 14px rgba(12,24,48,0.08); }}
					.header {{ background:#0b5cff; color:#ffffff; padding:16px; text-align:center; }}
					.content {{ padding:24px; line-height:1.45; }}
					.h1 {{ margin:0 0 12px 0; font-size:20px; }}
					.button {{ display:inline-block; background:#0b5cff; color:#ffffff; padding:12px 18px; border-radius:6px; text-decoration:none; font-weight:600; }}
					.small {{ font-size:13px; color:#6b7280; }}
					.footer {{ padding:16px; font-size:12px; color:#777777; text-align:center; }}
					.preheader {{ display:none !important; visibility:hidden; opacity:0; height:0; width:0; }}
					a {{ color:#0b5cff; }}
				</style>
			</head>
			<body>
				<span class='preheader'>{WebUtility.HtmlEncode(preheader)}</span>
				<div class='container'>
					<div class='header'>
						<h1 class='h1'>StudeeHub</h1>
					</div>
					<div class='content'>
						{bodyHtml}
					</div>
					<div class='footer small'>
						<p>If you didn't expect this email, you can safely ignore it.</p>
						<p>StudeeHub Team</p>
					</div>
				</div>
			</body>
			</html>";
		}

		public string ScheduleReminderTemplate(string fullName, string title, DateTime startTime)
		{
			var safeName = WebUtility.HtmlEncode(fullName);
			var safeTitle = WebUtility.HtmlEncode(title);
			var when = WebUtility.HtmlEncode(startTime.ToString("yyyy-MM-dd HH:mm"));

			var body = $@"
				<h2 style='margin-top:0;'>Hi {safeName},</h2>
				<p>This is a reminder for your upcoming session:</p>
				<ul>
					<li><strong>Title:</strong> {safeTitle}</li>
					<li><strong>Starts:</strong> {when}</li>
				</ul>
				<p style='margin:18px 0;'>Tip: Close distractions and try a focused Pomodoro session.</p>
				<p style='text-align:center; margin-top:20px;'><a class='button' href='#'>Open Session</a></p>";

			return WrapHtml($"Reminder: {title} at {startTime:HH:mm}", $"Reminder — {title}", body);
		}

		public string ScheduleCheckinTemplate(string fullName, string title)
		{
			var safeName = WebUtility.HtmlEncode(fullName);
			var safeTitle = WebUtility.HtmlEncode(title);

			var body = $@"
				<h2 style='margin-top:0;'>Hey {safeName},</h2>
				<p>It's time to check in for your session:</p>
				<p><strong>{safeTitle}</strong></p>
				<p>If you're ready, start your session now to keep the momentum going.</p>
				<p style='text-align:center; margin-top:20px;'><a class='button' href='#'>Check in</a></p>";

			return WrapHtml($"Check in for {title}", $"Check-in — {title}", body);
		}

		public string GetRegisterTemplate(string username, string verifyUrl)
		{
			var safeUsername = WebUtility.HtmlEncode(username);
			var safeVerifyUrl = WebUtility.HtmlEncode(verifyUrl);

			var body = $@"
				<h2 style='margin-top:0;'>Welcome, {safeUsername}!</h2>
				<p>Thanks for signing up. Please verify your email address to activate your account.</p>
				<p style='text-align:center; margin:24px 0;'><a class='button' href='{safeVerifyUrl}'>Verify your email</a></p>
				<p class='small'>If the button doesn't work, copy and paste the following link into your browser:<br/><a href='{safeVerifyUrl}'>{safeVerifyUrl}</a></p>";

			return WrapHtml("Please verify your email to complete registration", "Verify your StudeeHub account", body);
		}

		public string GetForgotPasswordTemplate(string username, string resetUrl)
		{
			var safeUsername = WebUtility.HtmlEncode(username);
			var safeResetUrl = WebUtility.HtmlEncode(resetUrl);

			var body = $@"
				<h2 style='margin-top:0;'>Hi {safeUsername},</h2>
				<p>We received a request to reset your password. Click the button below to continue. This link will expire shortly for security reasons.</p>
				<p style='text-align:center; margin:24px 0;'><a class='button' href='{safeResetUrl}'>Reset password</a></p>
				<p class='small'>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>";

			return WrapHtml("Reset your password", "Password reset instructions", body);
		}

		public string StreakReminderTemplate(string fullname)
		{
			var safeName = WebUtility.HtmlEncode(fullname);

			var body = $@"
				<h2 style='margin-top:0;'>Hi {safeName},</h2>
				<p>You haven't studied yet today — keep your streak alive!</p>
				<ul>
					<li>Try one 25-minute Pomodoro session</li>
					<li>Or review a short set of flashcards</li>
				</ul>
				<p style='text-align:center; margin-top:20px;'><a class='button' href='#'>Keep my streak</a></p>";

			return WrapHtml("Keep your streak alive", "Streak reminder", body);
		}

		public string ExpiredSubscriptionTemplate(string fullname, string planName, DateTime endDate)
		{
			var safeName = WebUtility.HtmlEncode(fullname);
			var safePlanName = WebUtility.HtmlEncode(planName);
			var safeEndDate = WebUtility.HtmlEncode(endDate.ToString("yyyy-MM-dd"));
			var body = $@"
				<h2 style='margin-top:0;'>Hi {safeName},</h2>
				<p>Your subscription to <strong>{safePlanName}</strong> expired on <strong>{safeEndDate}</strong>.</p>
				<p>To continue enjoying premium features, please renew your subscription.</p>
				<p style='text-align:center; margin-top:20px;'><a class='button' href='#'>Renew now</a></p>";
			return WrapHtml("Your subscription has expired", "Subscription expired", body);
		}
	}
}
