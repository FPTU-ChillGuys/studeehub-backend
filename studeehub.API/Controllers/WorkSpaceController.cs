using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.WorkSpace;
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
		[Authorize(Roles = "user")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateWorkSpace([FromBody] CreateWorkSpaceRequest requests)
		{
			var result = await _workSpaceService.CreateWorkSpaceAsync(requests);
			return result.Success ? Ok(result) : BadRequest(result);
		}
	}
}
