using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkSpaceController : ControllerBase
	{
		private readonly IWorkSpaceService _workSpaceService;

		public WorkSpaceController(IWorkSpaceService workSpaceService)
		{
			_workSpaceService = workSpaceService;
		}

		[HttpPost]
		//[Authorize(Roles = "user")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateWorkSpace([FromBody] CreateWorkSpaceRequest requests)
			=> await _workSpaceService.CreateWorkSpaceAsync(requests);

		[HttpPut("{id:Guid}")]
		//[Authorize]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateWorkSpace([FromRoute] Guid id, [FromBody] UpdateWorkSpaceRequest requests)
			=> await _workSpaceService.UpdateWorkSpaceAsync(id, requests);

		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteWorkSpace([FromRoute] Guid id)
			=> await _workSpaceService.DeleteWorkSpaceAsync(id);
	}
}
