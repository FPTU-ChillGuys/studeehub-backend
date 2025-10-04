using studeehub.Domain.Enums;

namespace studeehub.Application.DTOs.Requests.Streak
{
	public class CreateStreakRequest
	{
		public Guid UserId { get; set; }
		public StreakType Type { get; set; }
	}
}
