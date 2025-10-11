using Net.payOS.Types;

namespace studeehub.Application.DTOs.Requests.PayOS
{
	public sealed record CreatePaymentLinkRequest
	{
		public List<ItemData> Items { get; init; } = null!;
		public string? Description { get; init; }
		public string ReturnUrl { get; init; } = null!;
		public string CancelUrl { get; init; } = null!;
		public string? BuyerName { get; init; }
		public string? BuyerEmail { get; init; }
		public string? BuyerPhone { get; init; }
		public string? BuyerAddress { get; init; }
		public long? ExpiredAt { get; init; }

		// Optional: create subscription/payment transaction for a user when creating the PayOS link.
		// Provide both UserId and SubscriptionPlanId to enable subscription creation.
		public Guid? UserId { get; init; }
		public Guid? SubscriptionPlanId { get; init; }
	}
}
