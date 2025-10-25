using studeehub.Application.DTOs.Requests.Base;

namespace studeehub.Application.DTOs.Requests.User
{
	public class UpdateUserRequest : UploadFileRequest
	{
		public string? ProfilePictureUrl { get; set; }
		public string? FullName { get; set; }
		public string? Address { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
