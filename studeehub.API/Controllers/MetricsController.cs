using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MetricsController : ControllerBase
	{
		private readonly IUserMetricsService _userMetricsService;
		public MetricsController(IUserMetricsService userMetricsService)
		{
			_userMetricsService = userMetricsService;
		}

		[HttpGet("user/{id:Guid}/metrics")]
		[ProducesResponseType(typeof(BaseResponse<UserSelfMetricsResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<UserSelfMetricsResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<UserSelfMetricsResponse>> GetUserMetricsByIdAsync([FromRoute] Guid id, [FromQuery] GetUserSelfMetricsRequest request)
			=> await _userMetricsService.GetUserSelfMetricsAsync(id, request);

		[HttpGet("admin/metrics")]
		[ProducesResponseType(typeof(BaseResponse<UserMetricsResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<UserMetricsResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<UserMetricsResponse>> GetUserMetricsAsync([FromQuery] GetUserMetricsRequest request)
			=> await _userMetricsService.GetAdminMetricsAsync(request);
	}
}
