namespace studeehub.Application.DTOs.Requests.VnPay
{
	public class VnPaymentRequest
	{
		public Guid SubscriptionId { get; set; }
		public Guid PaymentTransactionId { get; set; }
		public int? Amount { get; set; }
	}
}
