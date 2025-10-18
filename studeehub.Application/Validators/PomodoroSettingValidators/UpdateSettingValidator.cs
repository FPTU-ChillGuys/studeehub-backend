using FluentValidation;
using studeehub.Application.DTOs.Requests.Pomodoro;

namespace studeehub.Application.Validators.PomodoroSettingValidators
{
	public class UpdateSettingValidator : AbstractValidator<UpdateSettingRequest>
	{
		public UpdateSettingValidator()
		{
			RuleFor(x => x.WorkDuration)
				.InclusiveBetween(15, 180).WithMessage("Work duration must be between 15 and 180 minutes.");
			RuleFor(x => x.ShortBreakDuration)
				.InclusiveBetween(5, 60).WithMessage("Short break duration must be between 5 and 60 minutes.");
			RuleFor(x => x.LongBreakDuration)
				.InclusiveBetween(10, 120).WithMessage("Long break duration must be between 10 and 120 minutes.");
			RuleFor(x => x.LongBreakInterval)
				.InclusiveBetween(2, 10).WithMessage("Long break interval must be between 2 and 10 cycles.");
			RuleFor(x => x.AutoStartNext)
				.NotNull().WithMessage("AutoStartNext must be specified.");
		}
	}
}
