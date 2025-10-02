using studeehub.Domain.Enums;

namespace studeehub.Domain.Entities
{
	public class Subscription
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }

		// switched from string to enum for clear allowed values
		public SubscriptionType Type { get; set; } = SubscriptionType.Monthly;
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
		public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

		public virtual User User { get; set; } = null!;
	}
}
