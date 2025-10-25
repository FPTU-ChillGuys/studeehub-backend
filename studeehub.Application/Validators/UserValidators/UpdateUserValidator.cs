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
			RuleFor(x => x.ProfilePictureUrl)
				.MaximumLength(600).WithMessage("Profile picture URL must not exceed 600 characters.")
				.When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl));
		}
	}
}
