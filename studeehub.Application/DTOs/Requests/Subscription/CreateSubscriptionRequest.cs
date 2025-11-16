using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Application.DTOs.Requests.Subscription
{
	public class CreateSubscriptionRequest
	{
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }
		public SubscriptionStatus Status { get; set; }
	}
}
