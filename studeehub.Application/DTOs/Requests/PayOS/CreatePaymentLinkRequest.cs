namespace studeehub.Application.DTOs.Requests.PayOS
{
	public sealed record CreatePaymentLinkRequest
	{
		public string? Description { get; init; }
		public string ReturnUrl { get; init; } = null!;
		public string CancelUrl { get; init; } = null!;

		// Optional: create subscription/payment transaction for a user when creating the PayOS link.
		// Provide both UserId and SubscriptionPlanId to enable subscription creation.
		public Guid UserId { get; init; }
		public Guid SubscriptionPlanId { get; init; }
		public Guid? SubscriptionId { get; init; }
	}
}
