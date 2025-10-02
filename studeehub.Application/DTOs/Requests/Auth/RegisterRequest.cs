namespace studeehub.Application.DTOs.Requests.Auth
{
	public class RegisterRequest
	{
		public string UserName { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ClientUri { get; set; } = string.Empty; //https://localhost:7114/api/auth/verify-email
	}
}
