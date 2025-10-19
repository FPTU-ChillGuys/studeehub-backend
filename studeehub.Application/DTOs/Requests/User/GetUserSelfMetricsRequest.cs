namespace studeehub.Application.DTOs.Requests.User
{
	public class GetUserSelfMetricsRequest
	{
		// Optional: allow filtering by time range (month, week)
		public string? Period { get; set; } = "month";
	}
}
