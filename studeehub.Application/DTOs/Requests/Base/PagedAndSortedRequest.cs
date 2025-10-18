namespace studeehub.Application.DTOs.Requests.Base
{
	public class PagedAndSortedRequest
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;

		public string? SortBy { get; set; }
		public bool SortDescending { get; set; } = false;
	}
}
