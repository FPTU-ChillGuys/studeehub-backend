using FluentValidation;
using studeehub.Application.DTOs.Requests.PayOS;

namespace studeehub.Application.Validators.PayOsValidators
{
	public class CreatePaymentLinkRequestValidator : AbstractValidator<CreateLinkRequest>
	{
		public CreatePaymentLinkRequestValidator()
		{
			RuleFor(x => x.Description)
				.MaximumLength(25).WithMessage("Description must be at most 25 characters.");

			RuleFor(x => x.ReturnUrl)
				.NotEmpty().WithMessage("ReturnUrl is required.")
				.Must(BeAValidUrl).WithMessage("ReturnUrl must be a valid absolute URL.");

			RuleFor(x => x.CancelUrl)
				.NotEmpty().WithMessage("CancelUrl is required.")
				.Must(BeAValidUrl).WithMessage("CancelUrl must be a valid absolute URL.");

			RuleFor(x => new { x.UserId, x.SubscriptionPlanId })
				.Must(x => (x.UserId.HasValue && x.SubscriptionPlanId.HasValue) || (!x.UserId.HasValue && !x.SubscriptionPlanId.HasValue))
				.WithMessage("Both UserId and SubscriptionPlanId must be provided together or both must be null.");
		}

		private static bool BeAValidUrl(string? value)
		{
			if (string.IsNullOrWhiteSpace(value)) return false;
			return Uri.TryCreate(value, UriKind.Absolute, out _);
		}
	}
}
