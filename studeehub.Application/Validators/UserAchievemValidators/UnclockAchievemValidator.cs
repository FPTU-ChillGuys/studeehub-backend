using FluentValidation;
using studeehub.Application.DTOs.Requests.UserAchievem;

namespace studeehub.Application.Validators.UserAchievemValidators
{
	public class UnclockAchievemValidator : AbstractValidator<UnlockAchivemRequest>
	{
		public UnclockAchievemValidator()
		{
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId is required.")
				.NotEqual(Guid.Empty).WithMessage("UserId cannot be an empty GUID.");
			RuleFor(x => x.AchievementId)
				.NotEmpty().WithMessage("AchievementId is required.")
				.NotEqual(Guid.Empty).WithMessage("AchievementId cannot be an empty GUID.");
		}
	}
}
