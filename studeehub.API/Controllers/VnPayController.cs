using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.PaymentTransaction;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/payments/vnpay")]
	[ApiController]
	public class VnPayController : ControllerBase
	{
		private readonly IPayTransactionService _payTransactionService;

		public VnPayController(IPayTransactionService payTransactionService)
		{
			_payTransactionService = payTransactionService;
		}

		// POST /api/payments/vnpay
		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreatePaymentSession([FromBody] CreatePaymentSessionRequest request)
			=> await _payTransactionService.CreatePaymentSessionAsync(request, HttpContext);

		// GET /api/payments/vnpay/callback
		[HttpGet("callback")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> VnPayCallback()
			=> await _payTransactionService.HandleVnPayCallbackAsync(Request.Query);
	}
}
