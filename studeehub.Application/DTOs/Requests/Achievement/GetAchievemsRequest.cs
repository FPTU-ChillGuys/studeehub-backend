using studeehub.Application.DTOs.Requests.Base;
using studeehub.Domain.Enums.Achievements;

namespace studeehub.Application.DTOs.Requests.Achievement
{
	public class GetAchievemsRequest : PagedAndSortedRequest
	{
		public string? SearchTerm { get; set; }
		public Guid? UserId { get; set; }
		public ConditionType? ConditionType { get; set; }
		public RewardType? RewardType { get; set; }
	}
}
