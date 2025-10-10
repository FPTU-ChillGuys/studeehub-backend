using studeehub.Domain.Enums.Streaks;

namespace studeehub.Application.DTOs.Responses.Streak
{
	public class GetStreakResponse
	{
		public Guid Id { get; set; }

		public StreakType Type { get; set; }
		public int CurrentCount { get; set; } = 0;
		public int LongestCount { get; set; } = 0;
		public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
