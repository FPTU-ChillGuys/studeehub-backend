using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Application.DTOs.Responses.Subscription
{
	public class GetSubscriptionResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionStatus Status { get; set; }

		public PlanDto SubscriptionPlan { get; set; } = null!;
		public UserDto User { get; set; } = null!;
	}

	public class PlanDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public decimal Price { get; set; }
		public int DurationInDays { get; set; }
	}

	public class UserDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = null!;
		public string Email { get; set; } = null!;
	}
}
