using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.API.Controllers
{
	[Route("api/subscriptions")]
	[ApiController]
	public class SubscriptionsController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionService;

		public SubscriptionsController(ISubscriptionService subscriptionService)
		{
			_subscriptionService = subscriptionService;
		}

		[HttpGet]
		public async Task<PagedResponse<GetSubscriptionResponse>> GetAllSubscriptionsAsync([FromQuery] GetPagedAndSortedSubscriptionsRequest request)
			=> await _subscriptionService.GetAllSubscriptionsAsync(request);

		// GET /api/users/{userId}/subscriptions
		[HttpGet("/api/users/{userId:Guid}/subscriptions")]
		[ProducesResponseType(typeof(BaseResponse<List<GetUserSubscriptionResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetUserSubscriptionResponse>>> GetSubscriptionByUserIdAsync([FromRoute] Guid userId)
			=> await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

		[HttpGet("/api/users/{userId:Guid}/subscriptions/active")]
		[ProducesResponseType(typeof(BaseResponse<GetUserSubscriptionResponse>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<GetUserSubscriptionResponse>> GetActiveSubscriptionByUserIdAsync([FromRoute] Guid userId)
			=> await _subscriptionService.GetActiveSubscriptionByUserIdAsync(userId);

		[HttpPost]
		public async Task<BaseResponse<string>> CreateSubscriptionAsync([FromBody] CreateSubscriptionRequest request)
			=> await _subscriptionService.CreateSubscriptionAsync(request);

		[HttpPut("{subscriptionId:Guid}/status")]
		public async Task<BaseResponse<string>> UpdateSubscriptionStatusAsync([FromRoute] Guid subscriptionId, [FromBody] SubscriptionStatus status)
			=> await _subscriptionService.UpdateSubscriptionStatusAsync(subscriptionId, status);
	}
}
