using FluentValidation;
using studeehub.Application.DTOs.Requests.Subscription;

namespace studeehub.Application.Validators.SubscriptionValidators
{
	public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionRequest>
	{
		public CreateSubscriptionValidator()
		{
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId is required.");
			RuleFor(x => x.SubscriptionPlanId)
				.NotEmpty().WithMessage("SubscriptionPlanId is required.");
			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Invalid subscription status.");
		}
	}
}
