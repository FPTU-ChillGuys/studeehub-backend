namespace studeehub.Application.DTOs.Requests.User
{
	public class GetUserLookupRequest
	{
		public bool IsActiveOnly { get; set; } = true;
		public bool IncludeAdmins { get; set; } = false;
	}
}
