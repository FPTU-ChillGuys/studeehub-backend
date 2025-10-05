using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AchievementController : ControllerBase
	{
		private readonly IAchievementService _achievementService;

		public AchievementController(IAchievementService achievementService)
		{
			_achievementService = achievementService;
		}

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateAchievement([FromBody] CreateAchievemRequest request)
			=> await _achievementService.CreateAchievementAsync(request);

		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateAchievement([FromRoute] Guid id, [FromBody] UpdateAchievemRequest request)
			=> await _achievementService.UpdateAchievementAsync(id, request);
    }
}
