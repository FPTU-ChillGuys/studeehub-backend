using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface IPayOSService
	{
		//public Task<BaseResponse<PaymentLink>> GetPaymentInformationByOrderCode(long id);
		public Task<BaseResponse<CreatePaymentLinkResponse>> CreatePaymentLink(CreateLinkRequest request);
		//public Task<BaseResponse<PaymentLink>> CancelPaymentLink(long orderCode, string? cancellationReason);
		public Task<BaseResponse<WebhookData>> TransferHandler(Webhook body);
		public Task<BaseResponse<ConfirmWebhookResponse>> ConfirmWebHook(ConfirmWebhookRequest request);
	}
}
