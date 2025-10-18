namespace studeehub.Domain.Entities
{
	public class PomodoroSetting
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }

		// Core durations (in minutes)
		public int WorkDuration { get; set; } = 25;
		public int ShortBreakDuration { get; set; } = 5;
		public int LongBreakDuration { get; set; } = 15;

		// After how many work sessions do we take a long break?
		public int LongBreakInterval { get; set; } = 4;

		// Auto-generation preferences
		public bool AutoStartNext { get; set; } = true;

		// Navigation
		public virtual User User { get; set; } = null!;
	}
}
