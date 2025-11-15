using Microsoft.AspNetCore.Mvc;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.API.Controllers
{
	[Route("api")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly IPayOSService _payOSService;
		public PaymentsController(IPayOSService payOSService)
		{
			_payOSService = payOSService;
		}

		// Return URLs used by PayOS redirect flows
		[HttpGet("payments/payos/success")]
		public async Task<BaseResponse<string>> Success()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment successful"));

		[HttpGet("payments/payos/cancel")]
		public async Task<BaseResponse<string>> Cancel()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment cancelled"));

		// Webhook management and handlers
		[HttpPost("payments/payos/webhooks/confirm")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<ConfirmWebhookResponse>> ConfirmWebhook([FromBody] ConfirmWebhookRequest request)
			=> await _payOSService.ConfirmWebHook(request);

		[HttpPost("payments/payos/webhooks")]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<WebhookData>> NewTransferHandler([FromBody] Webhook body)
			=> await _payOSService.TransferHandler(body);

		[HttpPost("payos/transfer_handler")]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<WebhookData>> TransferHandler([FromBody] Webhook body)
			=> await _payOSService.TransferHandler(body);

		//// Cancel (state change) for a payment identified by orderCode
		//[HttpPatch("payments/payos/{orderCode:long}/cancel")]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status200OK)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status500InternalServerError)]
		//public async Task<BaseResponse<PaymentLink>> CancelPayment(long orderCode, [FromQuery] string? cancellationReason)
		//	=> await _payOSService.CancelPaymentLink(orderCode, cancellationReason);

		//// Get payment information by order code
		//[HttpGet("payments/payos/{orderCode:long}")]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status200OK)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(BaseResponse<PaymentLink>), StatusCodes.Status500InternalServerError)]
		//public async Task<BaseResponse<PaymentLink>> GetPaymentInformationByOrderCode(long orderCode)
		//	=> await _payOSService.GetPaymentInformationByOrderCode(orderCode);

		// Create a payment link
		[HttpPost("payments/payos/links")]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentLinkResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentLinkResponse>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentLinkResponse>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<CreatePaymentLinkResponse>> CreatePaymentLink([FromBody] CreateLinkRequest request)
			=> await _payOSService.CreatePaymentLink(request);
	}
}
