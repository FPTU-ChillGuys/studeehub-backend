namespace studeehub.Application.DTOs.Requests.UserAchievem
{
	public class UnlockAchivemRequest
	{
		public Guid UserId { get; set; }
		public Guid AchievementId { get; set; }
	}
}
