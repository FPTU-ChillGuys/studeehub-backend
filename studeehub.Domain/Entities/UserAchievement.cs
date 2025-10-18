namespace studeehub.Domain.Entities
{
	public class UserAchievement
	{
		public Guid UserId { get; set; }
		public Guid AchievementId { get; set; }

		public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
		public bool IsClaimed { get; set; } = false;

		public virtual User User { get; set; } = null!;
		public virtual Achievement Achievement { get; set; } = null!;
	}
}
