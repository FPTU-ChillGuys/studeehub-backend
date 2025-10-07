using FluentValidation;
using studeehub.Application.DTOs.Requests.Subscription;

namespace studeehub.Application.Validators.SubscriptionValidators
{
	public class CreateSubPlanValidator : AbstractValidator<CreateSubPlanRequest>
	{
		public CreateSubPlanValidator()
		{
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Subscription plan code is required.")
				.Matches("^[A-Z0-9_]+$").WithMessage("Subscription plan code must be uppercase letters, numbers, or underscores only.")
				.MaximumLength(50).WithMessage("Subscription plan code must not exceed 50 characters.");
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Subscription plan name is required.")
				.MaximumLength(100).WithMessage("Subscription plan name must not exceed 100 characters.");
			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Description is required.")
				.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
			RuleFor(x => x.Price)
				.NotEmpty().WithMessage("Price is required.")
				.GreaterThan(0).WithMessage("Price must be greater than zero.");
			RuleFor(x => x.DurationInDays)
				.NotEmpty().WithMessage("Duration in days is required.")
				.GreaterThan(0).WithMessage("Duration in days must be greater than zero.");
			RuleFor(x => x.Currency)
				.NotEmpty().WithMessage("Currency is required.")
				.Length(3).WithMessage("Currency must be a valid 3-letter ISO code.");
			RuleFor(x => x.MaxDocuments)
				.GreaterThanOrEqualTo(0).WithMessage("Max documents must be zero or a positive number.");
			RuleFor(x => x.MaxStorageMB)
				.GreaterThanOrEqualTo(0).WithMessage("Max storage in GB must be zero or a positive number.");
			RuleFor(x => x.HasAIAnalysis)
				.NotNull().WithMessage("AI analysis feature flag is required.");
		}
	}
}
