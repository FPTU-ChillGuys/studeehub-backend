using FluentValidation;
using studeehub.Application.DTOs.Requests.User;

namespace studeehub.Application.Validators.UserValidators
{
	public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
	{
		public UpdateUserValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name is required.")
				.MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
			RuleFor(x => x.Address)
				.MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
			RuleFor(x => x.PhoneNumber)
				.MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.")
				.Matches(@"(03|05|07|08|09|01[2|6|8|9])+([0-9]{8})").WithMessage("Phone number format is invalid.")
				.When(x => !string.IsNullOrEmpty(x.PhoneNumber));
			//RuleFor(x => x.UserName)
			//	.NotEmpty().WithMessage("Username is required.")
			//	.MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
			//RuleFor(x => x.Email)
			//	.NotEmpty().WithMessage("Email is required.")
			//	.EmailAddress().WithMessage("A valid email is required.")
			//	.MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
		}
	}
}
