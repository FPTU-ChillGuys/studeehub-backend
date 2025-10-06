namespace studeehub.Domain.Entities
{
	public class PaymentTransaction
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionId { get; set; }

		public string PaymentMethod { get; set; } = "VNPAY";
		public string TransactionCode { get; set; } = string.Empty; // e.g., vnp_TxnRef
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "VND";

		public string Status { get; set; } = "Pending"; // "Pending", "Success", "Failed"
		public string? ResponseCode { get; set; }
		public string? BankCode { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? CompletedAt { get; set; }

		public virtual User User { get; set; } = null!;
		public virtual Subscription Subscription { get; set; } = null!;
	}
}
