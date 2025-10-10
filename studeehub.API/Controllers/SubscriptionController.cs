using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubscriptionController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionService;
		public SubscriptionController(ISubscriptionService subscriptionService)
		{
			_subscriptionService = subscriptionService;
		}

		[HttpGet("user/{userId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<List<GetSubscriptionResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetSubscriptionResponse>>> GetSubscriptionByUserIdAsync(Guid userId)
			=> await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);
	}
}
