using FluentValidation;
using studeehub.Application.DTOs.Requests.Streak;

namespace studeehub.Application.Validators.StreakValidators
{
	public class CreateStreakValidator : AbstractValidator<CreateStreakRequest>
	{
		public CreateStreakValidator()
		{
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId is required.")
				.NotEqual(Guid.Empty).WithMessage("UserId cannot be an empty GUID.");
			RuleFor(x => x.Type)
				.NotEmpty().WithMessage("Streak type is required.")
				.IsInEnum().WithMessage("Invalid streak type.");
		}
	}
}
