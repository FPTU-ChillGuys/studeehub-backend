namespace studeehub.Application.DTOs.Requests.Schedule
{
	public class UpdateScheduleRequest
	{
		public string Title { get; set; } = string.Empty;
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int ReminderMinutesBefore { get; set; } = 15;
		public string Description { get; set; } = string.Empty;
	}
}
