namespace studeehub.Application.DTOs.Requests.User
{
	public class UpdateUserRequest
	{
		public string FullName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
	}
}
