using FluentValidation;
using studeehub.Application.DTOs.Requests.Subscription;

namespace studeehub.Application.Validators.SubscriptionValidators
{
	public class UpdateSubPlanValidator : AbstractValidator<UpdateSubPlanRequest>
	{
		public UpdateSubPlanValidator()
		{
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Subscription plan code is required.")
				.Matches("^[A-Z0-9_]+$").WithMessage("Subscription plan code must be uppercase letters, numbers, or underscores only.")
				.MaximumLength(50).WithMessage("Subscription plan code must not exceed 50 characters.");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Subscription plan name is required.")
				.MaximumLength(100).WithMessage("Subscription plan name must not exceed 100 characters.");

			RuleFor(x => x.Description)
				.MaximumLength(500).WithMessage("Subscription plan description must not exceed 500 characters.");

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0).WithMessage("Subscription plan price must be a non-negative value.");

			RuleFor(x => x.DurationInDays)
				.GreaterThan(0).WithMessage("Subscription plan duration must be greater than zero days.");

			// IsActive is a non-nullable bool on the DTO; no NotNull rule required

			RuleFor(x => x.DiscountPercentage)
				.InclusiveBetween(0.0f, 100.0f).WithMessage("Discount percentage must be between 0 and 100.");

			RuleFor(x => x.Currency)
				.NotEmpty().WithMessage("Currency is required.")
				.Length(3).WithMessage("Currency must be a valid 3-letter ISO code.");

			RuleFor(x => x.DocumentUploadLimitPerDay)
				.GreaterThanOrEqualTo(0).WithMessage("Max documents must be a non-negative value.");

			RuleFor(x => x.MaxStorageMB)
				.GreaterThanOrEqualTo(0).WithMessage("Max storage must be a non-negative value.");

			RuleFor(x => x.AIQueriesPerDay)
				.GreaterThanOrEqualTo(0).WithMessage("AI queries per day must be a non-negative value.");

			RuleFor(x => x.FlashcardCreationLimitPerDay)
				.GreaterThanOrEqualTo(0).WithMessage("Flashcard creation limit per day must be a non-negative value.");

			// HasAIAnalysis is a non-nullable bool on the DTO; no NotNull rule required
		}
	}
}
