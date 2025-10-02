using FluentValidation;
using studeehub.Application.DTOs.Requests.Note;

namespace studeehub.Application.Validators.NoteValidators
{
	public class UpdateNoteValidator : AbstractValidator<UpdateNoteRequest>
	{
		public UpdateNoteValidator()
		{
			RuleFor(x => x.Id).NotEmpty().WithMessage("Note ID is required.");
			RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
				.MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
			RuleFor(x => x.Content).MaximumLength(10000).WithMessage("Content cannot exceed 10,000 characters.");
		}
	}
}
