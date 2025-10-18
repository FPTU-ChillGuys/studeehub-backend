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
	public class AuthsController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthsController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		public async Task<BaseResponse<string>> Register([FromBody] RegisterRequest request)
			=> await _authService.RegisterAsync(request, "User");

		[HttpPost("register/admin")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
		public async Task<BaseResponse<string>> RegisterByAdmin([FromBody] RegisterRequest request, [FromQuery] string role = "Admin")
			=> await _authService.RegisterAsync(request, role);

		[HttpGet("verify-email")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
			=> await _authService.VerifyEmailAsync(email, token);

		[HttpPost("login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<TokenResponse>> Login([FromBody] LoginRequest request)
			=> await _authService.LoginAsync(request);

		[HttpPost("google-login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
		public async Task<BaseResponse<TokenResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
			=> await _authService.LoginWithGoogleAsync(request);

		[HttpPost("refresh-token")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
			=> await _authService.RefreshTokenAsync(request);

		[HttpPost("forgot-password")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<string>> ForgotPassword([FromBody] ForgotPasswordRequest request)
			=> await _authService.ForgotPasswordAsync(request);

		[HttpPost("reset-password")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		public async Task<BaseResponse<string>> ResetPassword([FromBody] ResetPasswordRequest request)
			=> await _authService.ResetPasswordAsync(request);
	}
}
