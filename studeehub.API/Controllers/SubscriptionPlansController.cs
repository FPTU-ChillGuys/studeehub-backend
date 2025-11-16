using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Requests.SubscriptionPlan;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.SubPlan;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/subscription-plans")]
	[ApiController]
	public class SubscriptionPlansController : ControllerBase
	{
		private readonly ISubPlanService _subPlanService;

		public SubscriptionPlansController(ISubPlanService subPlanService)
		{
			_subPlanService = subPlanService;
		}

		// GET /api/subscription-plans
		[HttpGet]
		[ProducesResponseType(typeof(BaseResponse<List<GetSubPlanResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetSubPlanResponse>>> GetAllSubPlans()
			=> await _subPlanService.GetAllSubPlansAsync();

		[HttpGet("lookup")]
		[ProducesResponseType(typeof(BaseResponse<List<GetSubPlanLookupResponse>>), StatusCodes.Status200OK)]
		public async Task<BaseResponse<List<GetSubPlanLookupResponse>>> GetSubPlanLookup([FromQuery] GetSubPlanLookupRequest request)
			=> await _subPlanService.GetSubPlanLookupAsync(request);

		// GET /api/subscription-plans/{id}
		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetSubPlanResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetSubPlanResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetSubPlanResponse>> GetSubPlanById([FromRoute] Guid id)
			=> await _subPlanService.GetSubPlanByIdAsync(id);

		// POST /api/subscription-plans
		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateSubPlan([FromBody] CreateSubPlanRequest request)
			=> await _subPlanService.CreateSubPlanAsync(request);

		// PUT /api/subscription-plans/{id}
		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateSubPlan([FromRoute] Guid id, [FromBody] UpdateSubPlanRequest request)
			=> await _subPlanService.UpdateSubPlanAsync(id, request);

		// DELETE /api/subscription-plans/{id}
		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteSubPlan([FromRoute] Guid id)
			=> await _subPlanService.DeleteSubPlanAsync(id);
	}
}
