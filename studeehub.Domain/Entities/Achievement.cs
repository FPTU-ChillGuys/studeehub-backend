using studeehub.Domain.Enums.Achievements;

namespace studeehub.Domain.Entities
{
	public class Achievement
	{
		public Guid Id { get; set; }
		public string Code { get; set; } = string.Empty; // e.g., "STREAK_7_DAYS"
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }

		public int ConditionValue { get; set; }
		public ConditionType ConditionType { get; set; }
		public RewardType RewardType { get; set; }
		public int RewardValue { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Activation flag (explicit). Default true.
		public bool IsActive { get; set; } = true;

		public bool IsDeleted { get; set; } = false;
		// Nullable DeletedAt so it's empty until the entity is actually soft-deleted.
		public DateTime? DeletedAt { get; set; }

		public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
	}
}
