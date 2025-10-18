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

			// Validate at runtime to ensure StartTime is not in the past
			RuleFor(x => x.StartTime)
				.NotEmpty().WithMessage("Start time is required")
				.Must(start => start > DateTime.UtcNow).WithMessage("Start time must be in the future");

			RuleFor(x => x.EndTime)
				.NotEmpty().WithMessage("End time is required")
				.GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");

			RuleFor(x => x.ReminderMinutesBefore)
				.GreaterThanOrEqualTo(0).WithMessage("Reminder minutes cannot be negative")
				.LessThanOrEqualTo(60).WithMessage("Reminders can only be sent up to 60 minutes in advance");
		}
	}
}
