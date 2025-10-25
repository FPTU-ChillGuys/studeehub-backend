namespace studeehub.Application.DTOs.Responses.User
{
	public class GetUserResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string? ProfilePictureUrl { get; set; }
		public string? PhoneNumber { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public bool IsActive { get; set; }
	}
}
