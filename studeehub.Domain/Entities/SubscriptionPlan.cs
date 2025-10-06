namespace studeehub.Domain.Entities
{
	public class SubscriptionPlan
	{
		public Guid Id { get; set; }

		public string Code { get; set; } = string.Empty; // e.g., "BASIC_MONTHLY", "PRO_YEARLY"
		public string Name { get; set; } = string.Empty; // Display name
		public string? Description { get; set; }

		public decimal Price { get; set; } // VND or USD
		public string Currency { get; set; } = "VND";
		public int DurationInDays { get; set; } // e.g., 30 for monthly, 365 for yearly

		public bool IsActive { get; set; } = true;

		// Features (optional)
		public int MaxDocuments { get; set; } = 0;
		public int MaxStorageMB { get; set; } = 0;
		public bool HasAIAnalysis { get; set; } = true;

		public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
	}
}
