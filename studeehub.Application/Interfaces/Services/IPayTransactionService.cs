using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;

namespace studeehub.Application.Interfaces.Services
{
	public interface IPayTransactionService
	{
		public Task<BaseResponse<List<GetPayTXNResponse>>> GetPayTransactionsBySubscriptionId(Guid subscriptionId);
	}
}
