using studeehub.Application.DTOs.Requests.Base;
using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Application.DTOs.Requests.Subscription
{
	public class GetPagedAndSortedSubscriptionsRequest : PagedAndSortedRequest
	{
		public Guid? UserId { get; set; }
		public Guid? SubscriptionPlanId { get; set; }
		public SubscriptionStatus? Status { get; set; }

		// New filters
		public DateOnly? StartDateFrom { get; set; }
		public DateOnly? StartDateTo { get; set; }
		public DateOnly? EndDateFrom { get; set; }
		public DateOnly? EndDateTo { get; set; }

		// Search across user name/email and plan name
		public string? SearchTerm { get; set; }
	}
}
