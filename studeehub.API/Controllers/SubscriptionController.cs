using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubscriptionController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionService;
		private readonly IPayTransactionService _payTransactionService;
		public SubscriptionController(ISubscriptionService subscriptionService, IPayTransactionService payTransactionService)
		{
			_subscriptionService = subscriptionService;
			_payTransactionService = payTransactionService;
		}

		[HttpGet("user/{userId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<List<GetSubscriptionResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetSubscriptionResponse>>> GetSubscriptionByUserIdAsync(Guid userId)
			=> await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

		[HttpGet("{subscriptionId:Guid}/transactions")]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetPayTXNResponse>>> GetPaymentTransactionsBySubscriptionId(Guid subscriptionId)
			=> await _payTransactionService.GetPayTransactionsBySubscriptionId(subscriptionId);
	}
}
