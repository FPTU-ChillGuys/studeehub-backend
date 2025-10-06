namespace studeehub.Domain.Entities
{
	public class Schedule
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime? CheckInTime { get; set; }
		public bool IsCheckin { get; set; } = false;
		public int ReminderMinutesBefore { get; set; } = 15;
		public bool IsReminded { get; set; } = false;
		public string Description { get; set; } = string.Empty;
		public User User { get; set; } = null!;
	}
}
