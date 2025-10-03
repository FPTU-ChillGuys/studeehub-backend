using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Document;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace studeehub.Application.Services
{
	public class NoteService : INoteService
	{
		private readonly IGenericRepository<Note> _noteRepository;
		private readonly IDocumentService _documentService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateNoteRequest> _createNoteValidator;
		private readonly IValidator<UpdateNoteRequest> _updateNoteValidator;

		public NoteService(IGenericRepository<Note> noteRepository, IDocumentService documentService, IMapper mapper, IValidator<CreateNoteRequest> createNoteValidator, IValidator<UpdateNoteRequest> updateNoteValidator)
		{
			_noteRepository = noteRepository;
			_documentService = documentService;
			_mapper = mapper;
			_createNoteValidator = createNoteValidator;
			_updateNoteValidator = updateNoteValidator;
		}

        public async Task<BaseResponse<string>> BecomeDocumentAsync(Guid noteId)
        {
            // 1. Load note
            var note = await _noteRepository.GetByIdAsync(n => n.Id == noteId);
            if (note == null)
                return BaseResponse<string>.Fail("Note not found.", ErrorType.NotFound);

            // 2. Render markdown
            var sb = new StringBuilder();
            sb.AppendLine($"# {note.Title}");
            sb.AppendLine();
            sb.AppendLine(note.Content ?? string.Empty);
            var mdBytes = Encoding.UTF8.GetBytes(sb.ToString());

            // 3. Upload Markdown file (storage will handle unique naming)
            await using var ms = new MemoryStream(mdBytes);
            var fileName = $"{note.Title}.md"; // storage service will sanitize/rename as needed

            var uploadResult = await _documentService.UploadDocumentAsync(ms, fileName, "text/markdown; charset=utf-8");

            if (!uploadResult.Success || uploadResult.Data == null)
            {
                return BaseResponse<string>.Fail(
                    $"File upload failed: {uploadResult.Message ?? "unknown error"}",
                    ErrorType.ServerError
                );
            }

            // 4. Create document record
            var uploaded = uploadResult.Data;

            var createRequest = new CreateDocumentRequest
            {
                OwnerId = note.UserId,
                WorkSpaceId = note.WorkSpaceId,
                Name = $"{note.Title}.md", // Just use the original title, storage handles uniqueness
                Description = string.Empty,
                ContentType = string.IsNullOrEmpty(uploaded.ContentType) ? "text/markdown" : uploaded.ContentType,
                Url = uploaded.Url
            };

            var createResult = await _documentService.CreateDocumentAsync(createRequest);
            return createResult.Success
                ? BaseResponse<string>.Ok("Document created and note exported successfully")
                : BaseResponse<string>.Fail($"Failed to create document record: {createResult.Message}", ErrorType.ServerError);
        }

        public async Task<BaseResponse<string>> CreateNoteAsync(CreateNoteRequest request)
		{
			var validationResult = _createNoteValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, ErrorType.Validation);
			}

			try
			{
				var note = _mapper.Map<Note>(request);
				await _noteRepository.AddAsync(note);
				var result = await _noteRepository.SaveChangesAsync();

				return result
					? BaseResponse<string>.Ok("Note created successfully")
					: BaseResponse<string>.Fail("Failed to create Note", ErrorType.ServerError);
			}
			catch (Exception ex)
			{
				// If an ILogger is available in the future, log the exception here.
				return BaseResponse<string>.Fail(
					"An unexpected error occurred while creating the note.",
					ErrorType.ServerError,
					new List<string> { ex.Message }
				);
			}
		}

		public async Task<BaseResponse<string>> UpdateNoteAsync(Guid id, UpdateNoteRequest request)
		{
			var validationResult = _updateNoteValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var note = await _noteRepository.GetByIdAsync(n => n.Id == id);
			if (note == null)
			{
				return BaseResponse<string>.Fail("Note not found", ErrorType.NotFound);
			}

			var updatedNote = _mapper.Map(request, note);
			_noteRepository.Update(updatedNote);
			var result = await _noteRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Note updated successfully")
				: BaseResponse<string>.Fail("Failed to update note", ErrorType.ServerError);
		}
	}
}
