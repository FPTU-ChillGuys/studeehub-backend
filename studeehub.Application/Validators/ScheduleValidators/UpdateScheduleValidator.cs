using FluentValidation;
using studeehub.Application.DTOs.Requests.Schedule;

namespace studeehub.Application.Validators.ScheduleValidators
{
	public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleRequest>
	{
		public UpdateScheduleValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required")
				.MaximumLength(100).WithMessage("Title must not exceed 100 characters");
			RuleFor(x => x.Description)
				.MaximumLength(500).WithMessage("Description must not exceed 500 characters");
			RuleFor(x => x.StartTime)
				.NotEmpty().WithMessage("Start time is required");
			RuleFor(x => x.EndTime)
				.NotEmpty().WithMessage("End time is required")
				.GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
		}
	}
}
