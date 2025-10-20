using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[ProducesResponseType(typeof(PagedResponse<GetUserResponse>), StatusCodes.Status200OK)]
		public async Task<PagedResponse<GetUserResponse>> GetAllUsersAsync([FromQuery] GetPagedAndSortedUsersRequest request)
			=> await _userService.GetUsersAsync(request);

		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetUserResponse>> GetProfileByIdAsync([FromRoute] Guid id)
			=> await _userService.GetProfileByIdAsync(id);

		[HttpPatch("{id:Guid}/update-status")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<string>> UpdateStatusAsync([FromRoute] Guid id, [FromBody] bool request)
			=> await _userService.UpdateUserStatus(id, request);

		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateProfileAsync([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
			=> await _userService.UpdateProfileAsync(id, request);
	}
}
