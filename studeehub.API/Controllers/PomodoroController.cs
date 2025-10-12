using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PomodoroSession;
using studeehub.Application.DTOs.Responses.PomodoroSetting;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Enums.Pomodoros;

namespace studeehub.API.Controllers
{
	[Route("api")]
	[ApiController]
	public class PomodoroController : ControllerBase
	{
		private readonly IPomodoroSettingService _pomodoroSettingService;
		private readonly IPomodoroSessionService _pomodoroSessionService;
		public PomodoroController(IPomodoroSettingService pomodoroSettingService, IPomodoroSessionService pomodoroSessionService)
		{
			_pomodoroSettingService = pomodoroSettingService;
			_pomodoroSessionService = pomodoroSessionService;
		}

		// GET  /api/users/{userId}/pomodoro/sessions
		[HttpGet("users/{userId:guid}/pomodoro/sessions")]
		[ProducesResponseType(typeof(PagedResponse<GetSessionResponse>), StatusCodes.Status200OK)]
		public async Task<PagedResponse<GetSessionResponse>> GetSessions([FromRoute] Guid userId, [FromQuery] GetSessionsRequest request)
			=> await _pomodoroSessionService.GetSessionsAsync(userId, request);

		// GET  /api/users/{userId}/pomodoro/sessions/history
		[HttpGet("users/{userId:guid}/pomodoro/sessions/history")]
		[ProducesResponseType(typeof(PagedResponse<GetSessionHistoryResponse>), StatusCodes.Status200OK)]
		public async Task<PagedResponse<GetSessionHistoryResponse>> GetSessionsAndStats([FromRoute] Guid userId, [FromQuery] GetSessionsRequest request)
			=> await _pomodoroSessionService.GetSessionsAndStatsAsync(userId, request);

		// GET  /api/users/{userId}/pomodoro/settings
		[HttpGet("users/{userId:guid}/pomodoro/settings")]
		[ProducesResponseType(typeof(BaseResponse<GetSettingResponse>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<GetSettingResponse>> GetSettings([FromRoute] Guid userId)
			=> await _pomodoroSettingService.GetSettingByUserIdAsync(userId);

		// PUT  /api/pomodoro/settings/{settingId}
		[HttpPut("users/{userId:guid}/pomodoro/settings")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateSettings([FromRoute] Guid userId, [FromBody] UpdateSettingRequest request)
			=> await _pomodoroSettingService.UpdateAsync(userId, request);

		// POST /api/users/{userId}/pomodoro/sessions
		[HttpPost("users/{userId:guid}/pomodoro/sessions/start")]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetSessionResponse>> StartSession([FromRoute] Guid userId, [FromBody] PomodoroType type)
			=> await _pomodoroSessionService.StartSessionAsync(userId, type);

		// POST /api/users/{userId}/pomodoro/sessions/skip
		[HttpPost("users/{userId:guid}/pomodoro/sessions/skip")]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status400BadRequest)]
		public async Task<BaseResponse<GetSessionResponse>> SkipSession([FromRoute] Guid userId)
			=> await _pomodoroSessionService.SkipSessionAsync(userId);

		// POST /api/users/{userId}/pomodoro/sessions/complete
		[HttpPost("users/{userId:guid}/pomodoro/sessions/complete")]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetSessionResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetSessionResponse>> CompleteSession([FromRoute] Guid userId)
			=> await _pomodoroSessionService.CompleteSessionAsync(userId);
	}
}
