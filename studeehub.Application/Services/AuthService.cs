using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using studeehub.Application.DTOs.Requests.Auth;
using studeehub.Application.DTOs.Responses.Auth;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class AuthService(IValidator<ResetPasswordRequest> validator, IEmailTemplateService emailTemplateService, UserManager<User> userManager, IAuthRepository authRepository, IEmailService emailService) : IAuthService
	{
		private readonly IEmailTemplateService _templateService = emailTemplateService;
		private readonly UserManager<User> _userManager = userManager;
		private readonly IEmailService _emailService = emailService;
		private readonly IAuthRepository _authRepository = authRepository;
		private readonly IValidator<ResetPasswordRequest> _validator = validator;

		public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email!);
			if (user is null)
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var param = new Dictionary<string, string?>
			{
				{"token", token },
				{"email", request.Email!}
			};

			var callback = QueryHelpers.AddQueryString(request.ClientUri!, param);
			var emailContent = _templateService.GetForgotPasswordTemplate(user.UserName ?? "User", callback);

			await _emailService.SendEmailAsync(user.Email!, "Reset Password", emailContent);
			return BaseResponse<string>.Ok(token);
		}

		public async Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email!);
			if (user == null)
				return BaseResponse<TokenResponse>.Fail("User not found", Domain.Enums.ErrorType.NotFound);

			if (!user.IsActive)
				return BaseResponse<TokenResponse>.Fail("User is inactive", Domain.Enums.ErrorType.Forbidden);

			var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
			if (!isPasswordValid)
				return BaseResponse<TokenResponse>.Fail("Invalid password", Domain.Enums.ErrorType.Unauthorized);

			var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
			if (!isEmailConfirmed)
				return BaseResponse<TokenResponse>.Fail("Email not confirmed", Domain.Enums.ErrorType.Forbidden);

			var tokenResponse = await GenerateTokenResponseAsync(user);
			return BaseResponse<TokenResponse>.Ok(tokenResponse);
		}

		private async Task<TokenResponse> GenerateTokenResponseAsync(User user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault() ?? "User";
			return new TokenResponse()
			{
				AccessToken = await _authRepository.GenerateJwtToken(user, role),
				RefreshToken = await _authRepository.GenerateAndSaveRefreshToken(user)
			};
		}

		public async Task<BaseResponse<TokenResponse>> LoginWithGoogleAsync(GoogleLoginRequest request)
		{
			if (request is null || string.IsNullOrWhiteSpace(request.IdToken))
				return BaseResponse<TokenResponse>.Fail("Invalid request", Domain.Enums.ErrorType.Validation);

			GoogleJsonWebSignature.Payload payload;
			try
			{
				payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
			}
			catch
			{
				return BaseResponse<TokenResponse>.Fail("Invalid Google token", Domain.Enums.ErrorType.Validation);
			}

			var email = payload?.Email;
			if (string.IsNullOrWhiteSpace(email))
				return BaseResponse<TokenResponse>.Fail("Google token does not contain an email", Domain.Enums.ErrorType.Validation);

			User? user = null;
			try
			{
				user = await _userManager.FindByEmailAsync(email);
				if (user is null)
				{
					// Attempt registration via repository; repository returns a created user or null on failure.
					user = await _authRepository.RegisterViaGoogleAsync(payload);
					if (user is null)
						return BaseResponse<TokenResponse>.Fail("Failed to create user account from Google payload", Domain.Enums.ErrorType.ServerError);
				}
			}
			catch (Exception ex)
			{
				// Surface repository errors as friendly responses
				return BaseResponse<TokenResponse>.Fail("Google registration failed: " + ex.Message, Domain.Enums.ErrorType.ServerError);
			}

			if (!user.IsActive)
				return BaseResponse<TokenResponse>.Fail("Your account is inactive.", Domain.Enums.ErrorType.Forbidden);

			// Ensure email is confirmed (repository may have done this already for new users)
			if (!user.EmailConfirmed)
			{
				await _authRepository.ConfirmEmailAsync(user);
			}

			// No need for an unconditional double UpdateAsync; repository ConfirmEmailAsync already updates if needed.
			// Generate your own JWT + refresh token
			var tokenResponse = await GenerateTokenResponseAsync(user);

			var roles = await _userManager.GetRolesAsync(user);
			var message = roles.Any() ? "Google login successful" : "Google registration and login successful";

			return BaseResponse<TokenResponse>.Ok(tokenResponse, message);
		}

		public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
		{
			var user = await _authRepository.ValidateRefreshToken(request.UserId, request.RefreshToken);
			if (user is null)
				return BaseResponse<TokenResponse>.Fail("Invalid refresh token", Domain.Enums.ErrorType.Validation);

			var tokenResponse = await GenerateTokenResponseAsync(user);
			return BaseResponse<TokenResponse>.Ok(tokenResponse);
		}

		public async Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string role)
		{
			var existingUser = await _userManager.FindByEmailAsync(request.Email);
			if (existingUser != null)
				return BaseResponse<string>.Fail("Email already exists", Domain.Enums.ErrorType.Conflict);

			var user = new User
			{
				FullName = request.FullName,
				Email = request.Email,
				UserName = request.UserName,
				IsActive = true
			};

			var identityResult = await _userManager.CreateAsync(user, request.Password!);
			if (!identityResult.Succeeded)
				return BaseResponse<string>.Fail(
					"Failed to create user",
					Domain.Enums.ErrorType.Validation,
					identityResult.Errors.Select(e => e.Description).ToList()
				);

			var roleResult = await _userManager.AddToRoleAsync(user, role);
			if (!roleResult.Succeeded)
				return BaseResponse<string>.Fail(
					"Failed to assign role",
					Domain.Enums.ErrorType.Validation,
					roleResult.Errors.Select(e => e.Description).ToList()
				);

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			// Send verification email
			var verifyUrl = QueryHelpers.AddQueryString(
				request.ClientUri!,
				new Dictionary<string, string?>
				{
					{ "email", request.Email! },
					{ "token", token }
				}
			);

			var emailContent = _templateService.GetRegisterTemplate(
				request.FullName ?? request.Email,
				verifyUrl
			);

			await _emailService.SendEmailAsync(request.Email!, "Email Confirmation", emailContent);

			return BaseResponse<string>.Ok(token, "User registered successfully, Please check your email to verify!");

		}

		public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
		{
			var validationResult = await _validator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", Domain.Enums.ErrorType.Validation, errors);
			}

			var user = await _userManager.FindByEmailAsync(request.Email!);
			if (user is null)
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);

			var resetResult = await _userManager.ResetPasswordAsync(user, request.Token!, request.Password!);
			if (!resetResult.Succeeded)
				return BaseResponse<string>.Fail(
					"Failed to reset password",
					Domain.Enums.ErrorType.Validation,
					resetResult.Errors.Select(e => e.Description).ToList());

			return BaseResponse<string>.Ok("Password reset successfully");
		}

		public async Task<BaseResponse<string>> VerifyEmailAsync(string email, string token)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded)
			{
				return BaseResponse<string>.Fail("failed to verify", Domain.Enums.ErrorType.ServerError);
			}

			return BaseResponse<string>.Ok("Success");
		}
	}
}
