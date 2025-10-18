using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Base;
using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Schedule;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SchedulesController : ControllerBase
	{
		private readonly IScheduleService _scheduleService;

		public SchedulesController(IScheduleService scheduleService)
		{
			_scheduleService = scheduleService;
		}

		// GET /api/users/{userId}/schedules
		[HttpGet("/api/users/{userId:guid}/schedules")]
		[ProducesResponseType(typeof(PagedResponse<GetScheduleResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(PagedResponse<GetScheduleResponse>), StatusCodes.Status400BadRequest)]
		public async Task<PagedResponse<GetScheduleResponse>> GetSchedulesByUserId([FromRoute] Guid userId, [FromQuery] PagedAndSortedRequest request)
			=> await _scheduleService.GetSchedulesByUserIdAsync(userId, request);

		[HttpGet("{id:guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetScheduleResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetScheduleResponse>> GetScheduleById(Guid id)
			=> await _scheduleService.GetScheduleByIdAsync(id);

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateSchedule([FromBody] CreateScheduleRequest request)
			=> await _scheduleService.CreateScheduleAsync(request);

		[HttpPut("{id:guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateSchedule(Guid id, [FromBody] UpdateScheduleRequest request)
			=> await _scheduleService.UpdateScheduleAsync(id, request);

		[HttpPatch("{id:guid}/checkin")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CheckIn(Guid id)
			=> await _scheduleService.CheckIn(id);

		[HttpDelete("{id:guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteSchedule(Guid id)
			=> await _scheduleService.DeleteScheduleAsync(id);
	}
}
