using FluentValidation;
using studeehub.Application.DTOs.Requests.PayOS;

namespace studeehub.Application.Validators.PayOsValidators
{
	public class CreatePaymentLinkRequestValidator : AbstractValidator<CreatePaymentLinkRequest>
	{
		public CreatePaymentLinkRequestValidator()
		{
			RuleFor(x => x.Items)
				.NotEmpty().WithMessage("At least one item is required.")
				.Must(items => items != null && items.Count > 0).WithMessage("Items list cannot be null or empty.");

			RuleFor(x => x.Description)
				.MaximumLength(25).WithMessage("Description must be at most 25 characters.");

			RuleFor(x => x.ReturnUrl)
				.NotEmpty().WithMessage("ReturnUrl is required.")
				.Must(BeAValidUrl).WithMessage("ReturnUrl must be a valid absolute URL.");

			RuleFor(x => x.CancelUrl)
				.NotEmpty().WithMessage("CancelUrl is required.")
				.Must(BeAValidUrl).WithMessage("CancelUrl must be a valid absolute URL.");

			RuleFor(x => x.BuyerEmail)
				.EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.BuyerEmail))
				.WithMessage("BuyerEmail must be a valid email address.");

			RuleFor(x => x.BuyerPhone)
				.Matches(@"^\+?[0-9\s\-]{6,20}$")
				.When(x => !string.IsNullOrWhiteSpace(x.BuyerPhone))
				.WithMessage("BuyerPhone must contain only digits, spaces, dashes and optional leading + (6-20 chars).");

			RuleFor(x => x.ExpiredAt)
				.Must(v => v == null || (v > 0 && v <= int.MaxValue))
				.WithMessage("ExpiredAt must be a positive value within 32-bit integer range when provided.");
		}

		private static bool BeAValidUrl(string? value)
		{
			if (string.IsNullOrWhiteSpace(value)) return false;
			return Uri.TryCreate(value, UriKind.Absolute, out _);
		}
	}
}
