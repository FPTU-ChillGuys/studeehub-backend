namespace studeehub.Application.DTOs.Requests.User
{
	public class GetUserMetricsRequest
	{
		// Optional date range
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		// Or specify a shortcut type: "month", "week", "year"
		public string? Period { get; set; }

		// How to group chart data: "day", "week", "month"
		public string? GroupBy { get; set; } = "month";
	}
}
