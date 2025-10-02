namespace studeehub.Domain.Entities
{
	public class PomodoroSession
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }

		public DateTime Start { get; set; }
		public DateTime? End { get; set; }
		public TimeSpan? Duration => End.HasValue ? End - Start : null;

		public virtual User User { get; set; } = null!;
	}
}
