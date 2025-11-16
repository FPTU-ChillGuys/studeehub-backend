using FluentValidation;
using studeehub.Application.DTOs.Requests.PayOS;

namespace studeehub.Application.Validators.PayOsValidators
{
	public class CreatePaymentLinkRequestValidator : AbstractValidator<CreatePaymentLinkRequest>
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

			// When no SubscriptionId is supplied, require both UserId and SubscriptionPlanId to be provided (non-empty GUID)
			When(x => x.SubscriptionId == null, () =>
			{
				RuleFor(x => x.UserId)
					.Must(id => id != Guid.Empty).WithMessage("UserId is required when SubscriptionId is not provided.");

				RuleFor(x => x.SubscriptionPlanId)
					.Must(id => id != Guid.Empty).WithMessage("SubscriptionPlanId is required when SubscriptionId is not provided.");
			});
		}

		private static bool BeAValidUrl(string? value)
		{
			if (string.IsNullOrWhiteSpace(value)) return false;
			return Uri.TryCreate(value, UriKind.Absolute, out _);
		}
	}
}
