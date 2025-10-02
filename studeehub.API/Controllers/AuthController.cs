using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Auth;
using studeehub.Application.DTOs.Responses.Auth;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<BaseResponse<string>>> Register([FromBody] RegisterRequest request)
		{
			var result = await _authService.RegisterAsync(request, "User");
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPost("register/admin")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<BaseResponse<string>>> RegisterByAdmin([FromBody] RegisterRequest request, [FromQuery] string role = "Admin")
		{
			var result = await _authService.RegisterAsync(request, role);
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpGet("verify-email")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<BaseResponse<string>>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
		{
			var result = await _authService.VerifyEmailAsync(email, token);
			return result.Success ? Ok(result) : NotFound(result);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<BaseResponse<TokenResponse>>> Login([FromBody] LoginRequest request)
		{
			var result = await _authService.LoginAsync(request);
			return result.Success ? Ok(result) : Unauthorized(result);
		}

		[HttpPost("google-login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<BaseResponse<TokenResponse>>> GoogleLogin([FromBody] GoogleLoginRequest request)
		{
			var result = await _authService.LoginWithGoogleAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPost("refresh-token")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<BaseResponse<TokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var result = await _authService.RefreshTokenAsync(request);
			return result.Success ? Ok(result) : Unauthorized(result);
		}

		[HttpPost("forgot-password")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<BaseResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
		{
			var result = await _authService.ForgotPasswordAsync(request);
			return result.Success ? Ok(result) : NotFound(result);
		}

		[HttpPost("reset-password")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<BaseResponse<string>>> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			var result = await _authService.ResetPasswordAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
		}
	}
}
