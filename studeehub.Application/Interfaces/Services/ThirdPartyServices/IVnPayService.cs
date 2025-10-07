using Microsoft.AspNetCore.Http;
using studeehub.Application.DTOs.Requests.VnPay;
using studeehub.Application.DTOs.Responses.VnPay;

namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPaymentRequest requestModel);
		VnPaymentResponse PaymentExcute(IQueryCollection collections);
	}
}
