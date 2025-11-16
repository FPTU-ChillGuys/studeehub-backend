namespace studeehub.Application.DTOs.Responses.User
{
	public class GetUserLookupResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}
}
