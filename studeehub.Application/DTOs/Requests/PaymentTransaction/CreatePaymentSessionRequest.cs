namespace studeehub.Application.DTOs.Requests.PaymentTransaction
{
	public class CreatePaymentSessionRequest
	{
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }
	}
}
