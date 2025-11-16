using studeehub.Application.DTOs.Responses.SubPlan;
using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Application.DTOs.Responses.Subscription
{
	public class GetUserSubscriptionResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionStatus Status { get; set; }

		// Expose plan data via a DTO (do not return EF/domain entity directly)
		public GetSubPlanResponse SubscriptionPlan { get; set; } = null!;
	}

}
