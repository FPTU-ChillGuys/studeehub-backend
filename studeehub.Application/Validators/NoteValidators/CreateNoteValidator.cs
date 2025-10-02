using FluentValidation;
using studeehub.Application.DTOs.Requests.Note;

namespace studeehub.Application.Validators.NoteValidators
{
	public class CreateNoteValidator : AbstractValidator<CreateNoteRequest>
	{
		public CreateNoteValidator()
		{
			RuleFor(x => x.WorkSpaceId).NotEmpty().WithMessage("WorkSpaceId is required.");
			RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId is required.");
			RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
				.MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
		}
	}
}
