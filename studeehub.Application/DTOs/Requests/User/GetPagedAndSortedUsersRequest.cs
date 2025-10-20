using studeehub.Application.DTOs.Requests.Base;

namespace studeehub.Application.DTOs.Requests.User
{
	public class GetPagedAndSortedUsersRequest : PagedAndSortedRequest
	{
		public string? FullName { get; set; }
		public string? Address { get; set; }
		public bool? IsActive { get; set; }
		public DateOnly? CreatedFrom { get; set; }
		public DateOnly? CreatedTo { get; set; }
	}
}
