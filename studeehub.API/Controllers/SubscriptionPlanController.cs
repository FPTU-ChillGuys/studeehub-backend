using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubscriptionPlanController : ControllerBase
	{
		private readonly ISubPlanService _subPlanService;

		public SubscriptionPlanController(ISubPlanService subPlanService)
		{
			_subPlanService = subPlanService;
		}

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateSubPlan([FromBody] CreateSubPlanRequest request)
			=> await _subPlanService.CreateSubPlanAsync(request);

		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateSubPlan(Guid id, [FromBody] UpdateSubPlanRequest request)
			=> await _subPlanService.UpdateSubPlanAsync(id, request);

		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteSubPlan(Guid id)
			=> await _subPlanService.DeleteSubPlanAsync(id);
	}
}
