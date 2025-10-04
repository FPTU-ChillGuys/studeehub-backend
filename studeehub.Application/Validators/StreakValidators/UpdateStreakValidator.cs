using FluentValidation;
using studeehub.Application.DTOs.Requests.Streak;

namespace studeehub.Application.Validators.StreakValidators
{
	public class UpdateStreakValidator : AbstractValidator<UpdateStreakRequest>
	{
		public UpdateStreakValidator()
		{
			RuleFor(x => x.Type)
				.NotEmpty().WithMessage("Streak type is required.")
				.IsInEnum().WithMessage("Invalid streak type.");
		}
	}
}
