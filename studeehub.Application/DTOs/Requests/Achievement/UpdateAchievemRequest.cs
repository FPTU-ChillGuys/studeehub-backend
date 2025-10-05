using studeehub.Domain.Enums.Achievements;

namespace studeehub.Application.DTOs.Requests.Achievement
{
	public class UpdateAchievemRequest
	{
		public string Code { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }

		public int ConditionValue { get; set; }
		public ConditionType ConditionType { get; set; }
		public RewardType RewardType { get; set; }
		public int RewardValue { get; set; }

		public bool IsActive { get; set; }
	}
}
