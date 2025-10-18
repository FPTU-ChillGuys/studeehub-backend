using Net.payOS.Types;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IPayOSService
	{
		//public Task<BaseResponse<>
		public Task<BaseResponse<PaymentLinkInformation>> GetPaymentInformationByOrderCode(long id);
		public Task<BaseResponse<CreatePaymentResult>> CreatePaymentLink(CreatePaymentLinkRequest request);
		public Task<BaseResponse<PaymentLinkInformation>> CancelPaymentLink(long orderCode, string? cancellationReason);
		public Task<BaseResponse<WebhookData>> TransferHandler(WebhookType body);
		public Task<BaseResponse<string>> ConfirmWebHook(string webhookUrl);
	}
}
