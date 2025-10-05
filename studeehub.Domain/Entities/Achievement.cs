using studeehub.Domain.Enums.Achievements;

namespace studeehub.Domain.Entities
{
	public class Achievement
	{
		public Guid Id { get; set; }
		public string Code { get; set; } = string.Empty; // e.g., "STREAK_7_DAYS"
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }

		public int ConditionValue { get; set; } // e.g., 7
		public ConditionType ConditionType { get; set; } // enum, e.g., Streak, QuizCompleted
		public RewardType RewardType { get; set; } // enum, e.g., XP, Badge
		public int RewardValue { get; set; }

		public bool IsActive { get; set; } = true;

		public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
	}
}
