using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Domain.Entities
{
	public class Subscription
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public bool IsExpiryNotified { get; set; } = false;

		public virtual User User { get; set; } = null!;
		public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
		public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
	}
}
