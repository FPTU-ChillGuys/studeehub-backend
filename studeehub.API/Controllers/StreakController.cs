using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Streak;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Enums.Streaks;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StreakController : ControllerBase
	{
		private readonly IStreakService _streakService;

		public StreakController(IStreakService streakService)
		{
			_streakService = streakService;
		}

		[HttpGet("user/{userId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<List<GetStreakResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetStreakResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetStreakResponse>>> GetStreakByUserId(Guid userId, StreakType? type)
			=> await _streakService.GetStreakByUserIdAsync(userId, type);

		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetStreakResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetStreakResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetStreakResponse>> GetStreakById(Guid id)
			=> await _streakService.GetStreakByIdAsync(id);

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateStreak([FromBody] CreateStreakRequest request)
			=> await _streakService.CreateStreakAsync(request);

		[HttpPut("{userId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateStreak(Guid userId, [FromBody] UpdateStreakRequest request)
			=> await _streakService.UpdateStreakAsync(userId, request);
	}
}
