using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Achievement;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AchievementsController : ControllerBase
	{
		private readonly IAchievementService _achievementService;

		public AchievementsController(IAchievementService achievementService)
		{
			_achievementService = achievementService;
		}

		[HttpGet]
		[ProducesResponseType(typeof(PagedResponse<GetAchievemResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(PagedResponse<GetAchievemResponse>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(PagedResponse<GetAchievemResponse>), StatusCodes.Status404NotFound)]
		public async Task<PagedResponse<GetAchievemResponse>> GetPagedAchievements([FromQuery] GetAchievemsRequest request)
			=> await _achievementService.GetPagedAchievementsAsync(request);

		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetAchievemResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetAchievemResponse>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<GetAchievemResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetAchievemResponse>> GetAchievementById([FromRoute] Guid id)
			=> await _achievementService.GetAchievementByIdAsync(id);

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

		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteAchievement([FromRoute] Guid id)
			=> await _achievementService.DeleteAchievementAsync(id);
	}
}
