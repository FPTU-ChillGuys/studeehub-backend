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

			// 3. Upload Markdown file (avoid duplicates by generating unique filename and retrying)
			await using var ms = new MemoryStream(mdBytes);
			var baseName = SanitizeFileName(note.Title);
			const string contentType = "text/markdown; charset=utf-8";
			const int maxAttempts = 5;

			BaseResponse<UploadFileResponse>? uploadResult = null;
			string? uploadedFileName = null;

			for (int attempt = 0; attempt < maxAttempts; attempt++)
			{
				// generate readable unique suffix: timestamp + short guid
				var suffix = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
				var candidateName = $"{baseName}-{suffix}.md";

				// reset stream before each upload attempt
				ms.Position = 0;

				uploadResult = await _documentService.UploadDocumentAsync(ms, candidateName, contentType);

				if (uploadResult.Success && uploadResult.Data != null)
				{
					uploadedFileName = candidateName;
					break;
				}

				// If the service returns a message that indicates a duplicate or transient error,
				// we loop to try another unique name. If it returns a non-transient failure,
				// the loop will still retry but will fail after maxAttempts.
			}

			if (uploadResult == null || !uploadResult.Success || uploadResult.Data == null)
				return BaseResponse<string>.Fail(
					$"File upload failed after {maxAttempts} attempts: {uploadResult?.Message ?? "unknown error"}",
					ErrorType.ServerError
					);

			// 4. Create document record
			var uploaded = uploadResult.Data;

			var createRequest = new CreateDocumentRequest
			{
				OwnerId = note.UserId,
				WorkSpaceId = note.WorkSpaceId,
				Title = uploadedFileName ?? $"{baseName}.md",
				ContentType = string.IsNullOrEmpty(uploaded.ContentType) ? "text/markdown" : uploaded.ContentType,
				Url = uploaded.Url
			};

			var createResult = await _documentService.CreateDocumentAsync(createRequest);
			return createResult.Success
				? BaseResponse<string>.Ok("Document created and note exported successfully")
				: BaseResponse<string>.Fail($"Failed to create document record: {createResult.Message}", ErrorType.ServerError);
		}

		private static string SanitizeFileName(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return "New note";
			var sanitized = Regex.Replace(name, @"[^\w\-. ]", "_");
			return sanitized.Trim();
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

			var updatedNote = _mapper.Map<Note>(request);
			_noteRepository.Update(updatedNote);
			var result = await _noteRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Note updated successfully")
				: BaseResponse<string>.Fail("Failed to update note", ErrorType.ServerError);
		}
	}
}
