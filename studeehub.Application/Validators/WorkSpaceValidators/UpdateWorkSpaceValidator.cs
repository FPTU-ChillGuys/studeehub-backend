using FluentValidation;
using studeehub.Application.DTOs.Requests.WorkSpace;

namespace studeehub.Application.Validators.WorkSpaceValidators
{
	public class UpdateWorkSpaceValidator : AbstractValidator<UpdateWorkSpaceRequest>
	{
		public UpdateWorkSpaceValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
				.MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
			RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
		}
	}
}
