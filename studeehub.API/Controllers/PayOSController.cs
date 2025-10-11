using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PayOSController : ControllerBase
	{

		private readonly IPayOSService _payOSService;
		public PayOSController(IPayOSService payOSService)
		{
			_payOSService = payOSService;
		}

		[HttpGet("success")]
		public async Task<BaseResponse<string>> Success()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment successful"));

		[HttpGet("cancel")]
		public async Task<BaseResponse<string>> Cancel()
			=> await Task.FromResult(BaseResponse<string>.Ok("Payment cancelled"));

		[HttpPost("confirm-webhook")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> ConfirmWebhook(ConfirmWebhookRequest request)
			=> await _payOSService.ConfirmWebHook(request);

		[HttpPost("transfer_handler")]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<WebhookData>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<WebhookData>> TransferHandler(WebhookType body)
		 => await _payOSService.TransferHandler(body);

		[HttpPut("cancel/{orderCode:long}")]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<PaymentLinkInformation>> Cancel(long orderCode, [FromQuery] string cancellationReason)
		 => await _payOSService.CancelPaymentLink(orderCode, cancellationReason);

		[HttpGet("transaction-info/{orderCode:long}")]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<PaymentLinkInformation>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<PaymentLinkInformation>> GetPaymentInformationByOrderCode(long orderCode)
			=> await _payOSService.GetPaymentInformationByOrderCode(orderCode);

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<CreatePaymentResult>> CreatePayOSPaymentLink([FromBody] CreatePaymentLinkRequest request)
			=> await _payOSService.CreatePaymentLink(request);
	}
}
