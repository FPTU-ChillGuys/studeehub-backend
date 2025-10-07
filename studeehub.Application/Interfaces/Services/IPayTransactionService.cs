using Microsoft.AspNetCore.Http;
using studeehub.Application.DTOs.Requests.PaymentTransaction;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface IPayTransactionService
	{
		public Task<BaseResponse<string>> CreatePaymentSessionAsync(CreatePaymentSessionRequest request, HttpContext httpContext);
		public Task<BaseResponse<string>> HandleVnPayCallbackAsync(IQueryCollection query);
	}
}
