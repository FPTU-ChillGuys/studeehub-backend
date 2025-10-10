using System.Transactions;

namespace studeehub.Application.DTOs.Responses.PayTransaction
{
	public class GetPayTXNResponse
	{
		public Guid Id { get; set; }

		public string PaymentMethod { get; set; } = "VNPAY";
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "VND";

		public TransactionStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
	}
}
