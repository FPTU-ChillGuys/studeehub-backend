using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentTransactionsController : ControllerBase
	{
		private readonly IPayTransactionService _payTransactionService;

		public PaymentTransactionsController(IPayTransactionService payTransactionService)
		{
			_payTransactionService = payTransactionService;
		}

		// GET /api//paymenttransactions/{subscriptionId}
		[HttpGet("{subscriptionId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetPayTXNResponse>>> GetPaymentTransactionsBySubscriptionId([FromRoute] Guid subscriptionId)
			=> await _payTransactionService.GetPayTransactionsBySubscriptionId(subscriptionId);
	}
}
