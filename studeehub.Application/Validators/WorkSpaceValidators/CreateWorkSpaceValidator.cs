using FluentValidation;
using studeehub.Application.DTOs.Requests.WorkSpace;

namespace studeehub.Application.Validators.WorkSpaceValidators
{
	public class CreateWorkSpaceValidator : AbstractValidator<CreateWorkSpaceRequest>

	{
		public CreateWorkSpaceValidator()
		{
			RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId is required.");
			RuleFor(x => x.Name).MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
			RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
		}
	}
}
