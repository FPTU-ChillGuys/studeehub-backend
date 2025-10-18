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
				body {{ margin:0; padding:0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color:#2c3e50; }}
				.container {{ max-width:640px; margin:40px auto; background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 20px 60px rgba(0,0,0,0.3); }}
				.header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color:#ffffff; padding:40px 32px; text-align:center; position:relative; }}
				.header::after {{ content:''; position:absolute; bottom:0; left:0; right:0; height:4px; background: linear-gradient(90deg, #f093fb 0%, #f5576c 100%); }}
				.logo {{ font-size:32px; font-weight:700; letter-spacing:1px; margin:0; text-shadow:0 2px 4px rgba(0,0,0,0.1); }}
				.content {{ padding:48px 40px; line-height:1.8; font-size:16px; }}
				.greeting {{ margin:0 0 24px 0; font-size:28px; font-weight:600; color:#1a202c; letter-spacing:-0.5px; }}
				.message {{ margin:24px 0; color:#4a5568; }}
				.highlight {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); -webkit-background-clip:text; -webkit-text-fill-color:transparent; background-clip:text; font-weight:600; }}
				.info-box {{ background:#f7fafc; border-left:4px solid #667eea; padding:20px 24px; margin:24px 0; border-radius:8px; }}
				.info-box ul {{ margin:8px 0; padding-left:20px; }}
				.info-box li {{ margin:8px 0; color:#2d3748; }}
				.button {{ display:inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color:#ffffff; padding:16px 40px; border-radius:50px; text-decoration:none; font-weight:600; font-size:16px; box-shadow:0 10px 25px rgba(102,126,234,0.3); transition:transform 0.2s; margin:8px 0; }}
				.button:hover {{ transform:translateY(-2px); box-shadow:0 15px 35px rgba(102,126,234,0.4); }}
				.small {{ font-size:14px; color:#718096; line-height:1.6; }}
				.footer {{ background:#f7fafc; padding:32px 40px; font-size:13px; color:#718096; text-align:center; border-top:1px solid #e2e8f0; }}
				.footer-brand {{ font-weight:600; color:#4a5568; margin-bottom:8px; }}
				.preheader {{ display:none !important; visibility:hidden; opacity:0; height:0; width:0; }}
				.divider {{ height:1px; background: linear-gradient(90deg, transparent 0%, #e2e8f0 50%, transparent 100%); margin:32px 0; }}
				a {{ color:#667eea; text-decoration:none; }}
				a:hover {{ text-decoration:underline; }}
				strong {{ color:#2d3748; font-weight:600; }}
			</style>
		</head>
		<body>
			<span class='preheader'>{WebUtility.HtmlEncode(preheader)}</span>
			<div class='container'>
				<div class='header'>
					<h1 class='logo'>StudeeHub</h1>
				</div>
				<div class='content'>
					{bodyHtml}
				</div>
				<div class='footer'>
					<p class='footer-brand'>StudeeHub</p>
					<p>Tiếp sức hành trình học tập của bạn với các công cụ thông minh và trải nghiệm học tập cá nhân hóa.</p>
					<div class='divider' style='margin:20px auto; max-width:200px;'></div>
					<p class='small'>Nếu bạn không yêu cầu email này hoặc cho rằng nó được gửi nhầm, vui lòng bỏ qua thông báo này. Bảo mật tài khoản của bạn luôn là ưu tiên hàng đầu của chúng tôi.</p>
				</div>
			</div>
		</body>
		</html>";
		}

		public string ScheduleReminderTemplate(string fullName, string title, DateTime startTime)
		{
			var safeName = WebUtility.HtmlEncode(fullName);
			var safeTitle = WebUtility.HtmlEncode(title);
			var when = WebUtility.HtmlEncode(startTime.ToString("MMMM dd, yyyy 'at' HH:mm"));

			var body = $@"
			<h2 class='greeting'>Xin chào {safeName},</h2>
			<p class='message'>Đây là thư nhắc nhở về buổi học sắp tới của bạn. Chúng tôi trân trọng sự nỗ lực học tập liên tục của bạn và muốn đảm bảo bạn đã chuẩn bị sẵn sàng.</p>
			<div class='info-box'>
				<p style='margin-top:0; font-weight:600; color:#667eea; font-size:15px;'>THÔNG TIN BUỔI HỌC</p>
				<ul style='list-style:none; padding:0;'>
					<li style='margin:12px 0;'><strong>Tiêu đề buổi học:</strong> {safeTitle}</li>
					<li style='margin:12px 0;'><strong>Thời gian:</strong> {when}</li>
				</ul>
			</div>
			<p class='message'><span class='highlight'>Chuẩn bị đề xuất:</span> Chúng tôi khuyến nghị bạn giảm thiểu yếu tố gây xao nh distracting và cân nhắc sử dụng phương pháp Pomodoro để tối ưu hóa sự tập trung và hiệu suất trong buổi học.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='#'>Truy cập buổi học</a></p>
			<p class='small' style='text-align:center;'>Sự tận tâm học tập của bạn truyền cảm hứng cho chúng tôi. Chúng tôi luôn sẵn sàng hỗ trợ để bạn đạt thành tích học tập tốt.</p>";

			return WrapHtml($"Nhắc nhở buổi học sắp tới: {title}", $"Nhắc nhở buổi học — {title}", body);
		}

		public string ScheduleCheckinTemplate(string fullName, string title)
		{
			var safeName = WebUtility.HtmlEncode(fullName);
			var safeTitle = WebUtility.HtmlEncode(title);

			var body = $@"
			<h2 class='greeting'>Xin chào {safeName},</h2>
			<p class='message'>Đã đến lúc bắt đầu buổi học đã được lên lịch. Việc tham gia đều đặn của bạn thể hiện sự tận tâm xuất sắc đối với việc học.</p>
			<div class='info-box'>
				<p style='margin:0; font-size:18px; color:#2d3748;'><strong>{safeTitle}</strong></p>
			</div>
			<p class='message'>Chúng tôi khuyến khích bạn bắt đầu ngay để duy trì nhịp độ học tập và tận dụng tối đa lợi ích của lịch học có cấu trúc.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='#'>Bắt đầu buổi học</a></p>
			<p class='small' style='text-align:center; font-style:italic;'>Sự xuất sắc không phải là hành động, mà là thói quen. Sự cam kết của bạn hôm nay định hình thành công ngày mai.</p>";

			return WrapHtml($"Kiểm tra buổi học: {title}", $"Bắt đầu — {title}", body);
		}

		public string GetRegisterTemplate(string username, string verifyUrl)
		{
			var safeUsername = WebUtility.HtmlEncode(username);
			var safeVerifyUrl = WebUtility.HtmlEncode(verifyUrl);

			var body = $@"
			<h2 class='greeting'>Chào mừng đến StudeeHub, {safeUsername}</h2>
			<p class='message'>Chúng tôi rất vui được chào đón bạn vào cộng đồng người học năng động. Quyết định tham gia StudeeHub đánh dấu khởi đầu của một hành trình học tập được nâng cao.</p>
			<p class='message'>Để đảm bảo an toàn cho tài khoản và mở khóa toàn bộ tính năng của nền tảng, vui lòng xác thực địa chỉ email của bạn bằng cách nhấp vào nút bên dưới.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='{safeVerifyUrl}'>Xác thực địa chỉ email</a></p>
			<div class='divider'></div>
			<div class='info-box'>
				<p style='margin:0;'><strong>Tiếp theo là gì?</strong></p>
				<p class='small' style='margin:12px 0 0 0;'>Sau khi xác thực, bạn sẽ có quyền truy cập vào các công cụ học tập cá nhân hóa, theo dõi tiến độ, tính năng cộng tác và nhiều tài nguyên học tập cao cấp để nâng cao trải nghiệm học tập của mình.</p>
			</div>
			<p class='small' style='margin-top:24px;'>Nếu bạn gặp khó khăn với nút xác thực, vui lòng sao chép và dán liên kết sau vào trình duyệt của bạn:</p>
			<p class='small' style='word-break:break-all; background:#f7fafc; padding:12px; border-radius:6px; font-family:monospace;'><a href='{safeVerifyUrl}'>{safeVerifyUrl}</a></p>";

			return WrapHtml("Chào mừng đến StudeeHub — Vui lòng xác thực email", "Yêu cầu xác thực email", body);
		}

		public string GetForgotPasswordTemplate(string username, string resetUrl)
		{
			var safeUsername = WebUtility.HtmlEncode(username);
			var safeResetUrl = WebUtility.HtmlEncode(resetUrl);

			var body = $@"
			<h2 class='greeting'>Xin chào {safeUsername},</h2>
			<p class='message'>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản StudeeHub của bạn. Bảo mật tài khoản là ưu tiên hàng đầu và chúng tôi sẵn sàng hỗ trợ bạn khôi phục quyền truy cập.</p>
			<p class='message'>Để tiếp tục đặt lại mật khẩu, vui lòng nhấp vào nút an toàn bên dưới. Vì lý do bảo mật, liên kết này sẽ hết hạn sau một khoảng thời gian nhất định.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='{safeResetUrl}'>Đặt lại mật khẩu</a></p>
			<div class='divider'></div>
			<div class='info-box'>
				<p style='margin:0; color:#c53030;'><strong>⚠️ Thông báo bảo mật</strong></p>
				<p class='small' style='margin:12px 0 0 0;'>Nếu bạn không yêu cầu đặt lại mật khẩu này, vui lòng bỏ qua email. Tài khoản của bạn vẫn an toàn. Nếu bạn lo ngại về truy cập trái phép, hãy thay đổi mật khẩu ngay hoặc liên hệ đội hỗ trợ của chúng tôi.</p>
			</div>
			<p class='small' style='margin-top:24px; text-align:center; font-style:italic;'>Bảo mật tài khoản của bạn là trách nhiệm của chúng tôi. Chúng tôi cam kết bảo vệ dữ liệu học tập của bạn.</p>";

			return WrapHtml("Yêu cầu đặt lại mật khẩu đã được nhận", "Hướng dẫn đặt lại mật khẩu an toàn", body);
		}

		public string StreakReminderTemplate(string fullname)
		{
			var safeName = WebUtility.HtmlEncode(fullname);

			var body = $@"
			<h2 class='greeting'>Xin chào {safeName},</h2>
			<p class='message'>Chúng tôi nhận thấy hôm nay bạn chưa tham gia buổi học nào. Chuỗi học tập của bạn (<span class='highlight'>learning streak</span>) thể hiện sự kiên trì và nhất quán — những phẩm chất tạo nên người học xuất sắc.</p>
			<div class='info-box'>
				<p style='margin-top:0; font-weight:600; color:#667eea;'>HÀNH ĐỘNG ĐỀ XUẤT ĐỂ DUY TRÌ CHUỖI</p>
				<ul>
					<li>Hoàn thành một phiên Pomodoro tập trung 25 phút</li>
					<li>Ôn lại bộ flashcard được tuyển chọn để củng cố kiến thức</li>
					<li>Tham gia một hoạt động học ngắn phù hợp với mục tiêu của bạn</li>
				</ul>
			</div>
			<p class='message'>Hãy nhớ rằng sự nhất quán tạo nên kết quả theo thời gian. Ngay cả một buổi học ngắn hôm nay cũng đóng góp đáng kể cho thành công dài hạn của bạn.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='#'>Tiếp tục chuỗi học</a></p>
			<p class='small' style='text-align:center; font-style:italic;'>Thành công là tổng của những nỗ lực nhỏ lặp đi lặp lại mỗi ngày.</p>";

			return WrapHtml("Duy trì nhịp độ học tập của bạn", "Nhắc nhở chuỗi học tập", body);
		}

		public string UpcomingExpiryTemplate(string fullname, string planName, DateTime endDate)
		{
			var safeName = WebUtility.HtmlEncode(fullname);
			var safePlan = WebUtility.HtmlEncode(planName);
			var safeDate = WebUtility.HtmlEncode(endDate.ToString("MMMM dd, yyyy"));

			var body = $@"
				<h2 class='greeting'>Xin chào {safeName},</h2>
				<p class='message'>Gói <strong>{safePlan}</strong> của bạn sẽ hết hạn vào ngày <strong>{safeDate}</strong>.</p>
				<p>Hãy gia hạn sớm để không bị gián đoạn quyền truy cập vào các tính năng học tập nâng cao và giữ tiến độ học tập của bạn liền mạch.</p>
				<div class='divider'></div>
				<p style='text-align:center; margin:32px 0;'><a class='button' href='#'>Gia hạn ngay</a></p>
				<p class='small' style='text-align:center;'>Cảm ơn bạn đã đồng hành cùng StudeeHub. Hãy tiếp tục phát triển mỗi ngày!</p>";

			return WrapHtml("Gói đăng ký StudeeHub sắp hết hạn", "Lời nhắc gia hạn gói đăng ký", body);
		}

		public string ExpiredSubscriptionTemplate(string fullname, string planName, DateTime endDate)
		{
			var safeName = WebUtility.HtmlEncode(fullname);
			var safePlanName = WebUtility.HtmlEncode(planName);
			var safeEndDate = WebUtility.HtmlEncode(endDate.ToString("MMMM dd, yyyy"));

			var body = $@"
			<h2 class='greeting'>Xin chào {safeName},</h2>
			<p class='message'>Chúng tôi xin thông báo rằng gói <strong>{safePlanName}</strong> của bạn đã kết thúc vào ngày <strong>{safeEndDate}</strong>. Chúng tôi trân trọng sự ủng hộ của bạn và cơ hội được đồng hành cùng hành trình học tập của bạn.</p>
			<div class='info-box'>
				<p style='margin-top:0; font-weight:600; color:#667eea;'>TÍNH NĂNG PREMIUM BẠN SẼ BỎ LỠ</p>
				<ul class='small' style='margin:8px 0; padding-left:20px;'>
					<li>Phân tích nâng cao và theo dõi tiến độ</li>
					<li>Truy cập không giới hạn tài nguyên học tập cao cấp</li>
					<li>Hỗ trợ khách hàng ưu tiên</li>
					<li>Công cụ cộng tác nâng cao</li>
				</ul>
			</div>
			<p class='message'>Để khôi phục quyền truy cập vào các tính năng cao cấp và tiếp tục trải nghiệm học tập nâng cao, chúng tôi mời bạn gia hạn gói đăng ký khi thuận tiện.</p>
			<div class='divider'></div>
			<p style='text-align:center; margin:32px 0;'><a class='button' href='#'>Gia hạn đăng ký</a></p>
			<p class='small' style='text-align:center;'>Chúng tôi cam kết mang lại giá trị xuất sắc và hỗ trợ bạn đạt thành tích học tập. Cảm ơn bạn đã là thành viên quý giá của cộng đồng StudeeHub.</p>";

			return WrapHtml("Gói đăng ký StudeeHub của bạn đã hết hạn", "Thông báo gia hạn đăng ký", body);
		}

		public string SubscriptionActivatedTemplate(string fullName, string planName, DateTime endDate)
		{
			var safeName = WebUtility.HtmlEncode(fullName);
			var safePlan = WebUtility.HtmlEncode(planName);
			var formattedEnd = WebUtility.HtmlEncode(endDate.ToString("MMMM dd, yyyy"));

			var body = $@"
	<h2 class='greeting'>Chào mừng {safeName},</h2>

	<p class='message'>
		🎉 Chúc mừng! Gói <strong>{safePlan}</strong> của bạn đã được <span class='highlight'>kích hoạt thành công</span>.
	</p>

	<div class='info-box'>
		<p style='margin-top:0; font-weight:600; color:#667eea; font-size:15px;'>THÔNG TIN GÓI CỦA BẠN</p>
		<ul style='list-style:none; padding:0;'>
			<li style='margin:12px 0;'><strong>Tên gói:</strong> {safePlan}</li>
			<li style='margin:12px 0;'><strong>Ngày hết hạn:</strong> {formattedEnd}</li>
		</ul>
	</div>

	<p class='message'>
		Từ giờ bạn đã có quyền truy cập đầy đủ vào tất cả các tính năng cao cấp của <span class='highlight'>StudeeHub</span> —
		gồm AI Note Assistant, Flashcard thông minh, và hệ thống nhắc học tập tự động.
	</p>

	<div class='divider'></div>

	<p style='text-align:center; margin:32px 0;'>
		<a class='button' href='https://studeehub.app/dashboard'>
			Bắt đầu học ngay
		</a>
	</p>

	<p class='small' style='text-align:center;'>
		Hãy tận dụng sức mạnh của công cụ học tập cá nhân hóa để đạt hiệu quả tối đa.
		Cảm ơn bạn đã đồng hành cùng <strong>StudeeHub</strong>!
	</p>";

			return WrapHtml(
				$"Gói {planName} đã được kích hoạt thành công",
				$"Kích hoạt thành công — {planName}",
				body
			);
		}
	}
}
