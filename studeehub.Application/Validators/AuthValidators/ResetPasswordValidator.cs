using FluentValidation;
using studeehub.Application.DTOs.Requests.Auth;

namespace studeehub.Application.Validators.AuthValidators
{
	public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
	{
		public ResetPasswordValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.Token)
				.NotEmpty().WithMessage("Token is required.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

			RuleFor(x => x.ConfirmPassword)
				.NotEmpty().WithMessage("Confirm password is required.")
				.Equal(x => x.Password).WithMessage("Passwords do not match");
		}
	}
}
