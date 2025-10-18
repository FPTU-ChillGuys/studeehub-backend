using FluentValidation;
using studeehub.Application.DTOs.Requests.Achievement;

namespace studeehub.Application.Validators.AchievemValidators
{
	public class GetAchievemValidator : AbstractValidator<GetAchievemsRequest>
	{
		public GetAchievemValidator()
		{
			// PageNumber/PageSize are non-nullable in PagedAndSortedRequest; ensure they are positive
			RuleFor(x => x.PageNumber)
				.GreaterThan(0).WithMessage("Page number must be greater than 0.");

			RuleFor(x => x.PageSize)
				.GreaterThan(0).WithMessage("Page size must be greater than 0.");

			RuleFor(x => x.SortBy)
				.MaximumLength(50).WithMessage("SortBy cannot exceed 50 characters.")
				.When(x => !string.IsNullOrWhiteSpace(x.SortBy));

			// When a SortBy is supplied, SortDescending is allowed to be true/false (no extra validation needed).
		}
	}
}
