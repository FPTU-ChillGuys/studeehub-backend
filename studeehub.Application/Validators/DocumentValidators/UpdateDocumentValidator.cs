using FluentValidation;
using studeehub.Application.DTOs.Requests.Document;

namespace studeehub.Application.Validators.DocumentValidators
{
	public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentRequest>
    {
		public UpdateDocumentValidator()
		{
			RuleFor(RuleFor => RuleFor.Id).NotEmpty().WithMessage("DocumentId is required.");
			RuleFor(RuleFor => RuleFor.Title).NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        }
	}
}
