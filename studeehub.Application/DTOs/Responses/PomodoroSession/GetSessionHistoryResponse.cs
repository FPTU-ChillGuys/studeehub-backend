namespace studeehub.Application.DTOs.Responses.PomodoroSession
{
	public class GetSessionHistoryResponse
	{
		public List<GetSessionResponse> Sessions { get; set; } = new();
		public int TotalCount { get; set; }

		// Summary metrics for dashboard insights
		public double TotalFocusMinutes { get; set; }      // Total completed work time
		public double TotalBreakMinutes { get; set; }      // Total completed rest time
		public double AverageFocusDuration { get; set; }   // Avg work session duration
		public double CompletionRate { get; set; }         // Completed / Total sessions
	}
}
