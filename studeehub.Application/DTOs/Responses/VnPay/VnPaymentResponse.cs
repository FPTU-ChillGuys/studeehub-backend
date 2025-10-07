namespace studeehub.Application.DTOs.Responses.VnPay
{
	public class VnPaymentResponse
	{
		public bool Success { get; set; }
		public string OrderDescription { get; set; }
		public Guid SubscriptionId { get; set; }
		public Guid TransactionId { get; set; }
		public string TransactionNo { get; set; }
		public string Token { get; set; }
		public string VnPayResponseCode { get; set; }
		public string Message { get; set; }
		public decimal Amount { get; set; }
	}
}
