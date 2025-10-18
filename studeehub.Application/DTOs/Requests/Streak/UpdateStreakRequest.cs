using studeehub.Domain.Enums.Streaks;

namespace studeehub.Application.DTOs.Requests.Streak
{
	public class UpdateStreakRequest
	{
		public StreakType Type { get; set; }
		public bool IsActive { get; set; }
	}
}
