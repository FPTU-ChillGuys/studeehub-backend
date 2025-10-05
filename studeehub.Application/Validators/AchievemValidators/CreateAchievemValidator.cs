using FluentValidation;
using studeehub.Application.DTOs.Requests.Achievement;

namespace studeehub.Application.Validators.AchievemValidators
{
	public class CreateAchievemValidator : AbstractValidator<CreateAchievemRequest>
	{
		public CreateAchievemValidator()
		{
			RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
			RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
			RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
			RuleFor(x => x.ConditionType).NotEmpty().WithMessage("ConditionType is required.")
				.IsInEnum().WithMessage("ConditionType must be a valid enum value.");
			RuleFor(x => x.ConditionValue).GreaterThan(0).WithMessage("ConditionValue must be greater than 0.");
			RuleFor(x => x.RewardType).NotEmpty().WithMessage("RewardType is required.")
				.IsInEnum().WithMessage("RewardType must be a valid enum value.");
			RuleFor(x => x.RewardValue).GreaterThanOrEqualTo(0).WithMessage("RewardValue must be positive value.");
		}
	}
}
