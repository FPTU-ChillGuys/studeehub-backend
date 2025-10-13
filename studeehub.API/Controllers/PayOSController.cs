using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.API.Controllers
{
	[Route("api/payments/payos")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly IPayOSService _payOSService;
		public PaymentsController(IPayOSService payOSService)
		{
			_payOSService = payOSService;
		}

		// Return URLs used by PayOS redirect flows
		[HttpGet("success")]
		public async Task<BaseResponse<string>> Success()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment successful"));

		[HttpGet("cancel")]
		public async Task<BaseResponse<string>> Cancel()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment cancelled"));

		// Webhook management and handlers
		[HttpPost("webhooks/confirm")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> ConfirmWebhook([FromBody] string webhookUrl)
			=> await _payOSService.ConfirmWebHook(webhookUrl);

		[HttpPost("webhooks")]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<WebhookData>> TransferHandler([FromBody] WebhookType body)
			=> await _payOSService.TransferHandler(body);

		// Cancel (state change) for a payment identified by orderCode
		[HttpPatch("{orderCode:long}/cancel")]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<PaymentLinkInformation>> CancelPayment(long orderCode, [FromQuery] string? cancellationReason)
			=> await _payOSService.CancelPaymentLink(orderCode, cancellationReason);

		// Get payment information by order code
		[HttpGet("{orderCode:long}")]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<PaymentLinkInformation>> GetPaymentInformationByOrderCode(long orderCode)
			=> await _payOSService.GetPaymentInformationByOrderCode(orderCode);

		// Create a payment link
		[HttpPost("links")]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<CreatePaymentResult>> CreatePaymentLink([FromBody] CreatePaymentLinkRequest request)
			=> await _payOSService.CreatePaymentLink(request);
	}
}
