namespace studeehub.Application.DTOs.Responses.Schedule
{
	public class GetScheduleResponse
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime? CheckInTime { get; set; }
		public bool IsCheckin { get; set; } = false;
		public int ReminderMinutesBefore { get; set; } = 15;
		public string Description { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
