using FluentValidation;
using studeehub.Application.DTOs.Requests.Document;

namespace studeehub.Application.Validators.DocumentValidators
{
	public class CreateDocumentValidator : AbstractValidator<CreateDocumentRequest>
	{
		public CreateDocumentValidator()
		{
			RuleFor(x => x.WorkspaceId).NotEmpty().WithMessage("WorkSpaceId is required.");
			RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId is required.");
			RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
				.MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
			RuleFor(x => x.ContentType).NotEmpty().WithMessage("Type is required.");
			RuleFor(x => x.Url).NotEmpty().WithMessage("Path is required.");
		}
	}
}
