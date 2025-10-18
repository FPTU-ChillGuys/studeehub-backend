namespace studeehub.Application.DTOs.Requests.Auth
{
	public class LoginRequest
	{
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
