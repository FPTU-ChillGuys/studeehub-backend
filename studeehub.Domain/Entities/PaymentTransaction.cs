using studeehub.Domain.Enums.TransactionStatus;

namespace studeehub.Domain.Entities
{
	public class PaymentTransaction
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionId { get; set; }

		public string PaymentMethod { get; set; } = "VNPAY";
		public string TransactionCode { get; set; } = string.Empty;
		public string PaymentLinkId { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "VND";

		public string? CancellationReason { get; set; }
		public TransactionStatus Status { get; set; }
		public string? ResponseCode { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }

		public virtual User User { get; set; } = null!;
		public virtual Subscription Subscription { get; set; } = null!;
	}
}
