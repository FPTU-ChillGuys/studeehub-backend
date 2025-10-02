using studeehub.Application.DTOs.Requests.Auth;
using studeehub.Application.DTOs.Responses.Auth;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface IAuthService
	{
		Task<BaseResponse<string>> RegisterAsync(RegisterRequest register, string role);
		Task<BaseResponse<string>> VerifyEmailAsync(string email, string token);
		Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest login);
		Task<BaseResponse<TokenResponse>> LoginWithGoogleAsync(GoogleLoginRequest request);
		Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
		Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
		Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
	}
}
