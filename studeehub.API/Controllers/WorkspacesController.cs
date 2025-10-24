namespace studeehub.API.Controllers
{
	//[Route("api/[controller]")]
	//[ApiController]
	//public class WorkspacesController : ControllerBase
	//{
	//	private readonly IWorkspaceService _workSpaceService;

	//	public WorkspacesController(IWorkspaceService workSpaceService)
	//	{
	//		_workSpaceService = workSpaceService;
	//	}

	//	// GET /api/users/{userId}/workspaces
	//	[HttpGet("/api/users/{userId:Guid}/workspaces")]
	//	[ProducesResponseType(typeof(BaseResponse<List<GetWorkspaceResponse>>), StatusCodes.Status200OK)]
	//	[ProducesResponseType(typeof(BaseResponse<List<GetWorkspaceResponse>>), StatusCodes.Status404NotFound)]
	//	public async Task<BaseResponse<List<GetWorkspaceResponse>>> GetWorkSpacesByUserId([FromRoute] Guid userId)
	//		=> await _workSpaceService.GetWorkspacesByUserIdAsync(userId);

	//	[HttpGet("{id:Guid}")]
	//	[ProducesResponseType(typeof(BaseResponse<GetWorkspaceResponse>), StatusCodes.Status200OK)]
	//	[ProducesResponseType(typeof(BaseResponse<GetWorkspaceResponse>), StatusCodes.Status404NotFound)]
	//	public async Task<BaseResponse<GetWorkspaceResponse>> GetWorkSpaceById([FromRoute] Guid id)
	//		=> await _workSpaceService.GetWorkspaceByIdAsync(id);

	//	[HttpPost]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
	//	public async Task<BaseResponse<string>> CreateWorkSpace([FromBody] CreateWorkspaceRequest requests)
	//		=> await _workSpaceService.CreateWorkspaceAsync(requests);

	//	[HttpPut("{id:Guid}")]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
	//	public async Task<BaseResponse<string>> UpdateWorkSpace([FromRoute] Guid id, [FromBody] UpdateWorkspaceRequest requests)
	//		=> await _workSpaceService.UpdateWorkspaceAsync(id, requests);

	//	[HttpDelete("{id:Guid}")]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
	//	[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
	//	public async Task<BaseResponse<string>> DeleteWorkSpace([FromRoute] Guid id)
	//		=> await _workSpaceService.DeleteWorkspaceAsync(id);
	//}
}
