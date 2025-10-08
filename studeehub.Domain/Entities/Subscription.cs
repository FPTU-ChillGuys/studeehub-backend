using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Domain.Entities
{
	public class Subscription
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }

		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
		public SubscriptionStatus Status { get; set; } 

		public DateTime CreateAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
		public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
		public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
	}
}
