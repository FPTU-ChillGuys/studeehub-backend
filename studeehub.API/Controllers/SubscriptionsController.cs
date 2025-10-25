using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/subscriptions")]
	[ApiController]
	public class SubscriptionsController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionService;
		private readonly IPayTransactionService _payTransactionService;
		public SubscriptionsController(ISubscriptionService subscriptionService, IPayTransactionService payTransactionService)
		{
			_subscriptionService = subscriptionService;
			_payTransactionService = payTransactionService;
		}

		// GET /api/users/{userId}/subscriptions
		[HttpGet("/api/users/{userId:Guid}/subscriptions")]
		[ProducesResponseType(typeof(BaseResponse<List<GetSubscriptionResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetSubscriptionResponse>>> GetSubscriptionByUserIdAsync([FromRoute] Guid userId)
			=> await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

		[HttpGet("/api/users/{userId:Guid}/subscriptions/active")]
		[ProducesResponseType(typeof(BaseResponse<GetSubscriptionResponse>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<GetSubscriptionResponse>> GetActiveSubscriptionByUserIdAsync([FromRoute] Guid userId)
			=> await _subscriptionService.GetActiveSubscriptionByUserIdAsync(userId);

		// GET /api/subscriptions/{subscriptionId}/transactions
		[HttpGet("{subscriptionId:Guid}/transactions")]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetPayTXNResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetPayTXNResponse>>> GetPaymentTransactionsBySubscriptionId([FromRoute] Guid subscriptionId)
			=> await _payTransactionService.GetPayTransactionsBySubscriptionId(subscriptionId);
	}
}
