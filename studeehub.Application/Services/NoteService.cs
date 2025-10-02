using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
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

		public NoteService(IGenericRepository<Note> noteRepository, IDocumentService documentService, IMapper mapper, IValidator<CreateNoteRequest> createNoteValidator)
		{
			_noteRepository = noteRepository;
			_documentService = documentService;
			_mapper = mapper;
			_createNoteValidator = createNoteValidator;
		}

		public async Task<BaseResponse<string>> BecomeDocumentAsync(Guid noteId)
		{
			// 1. Load note
			var note = await _noteRepository.GetByIdAsync(n => n.Id == noteId);
			if (note == null)
				return BaseResponse<string>.Fail("Note not found.");

			// 2. Render markdown
			var sb = new StringBuilder();
			sb.AppendLine($"# {note.Title}");
			sb.AppendLine();
			sb.AppendLine(note.Content ?? string.Empty);
			var mdBytes = Encoding.UTF8.GetBytes(sb.ToString());

			// 3. Upload Markdown file
			await using var ms = new MemoryStream(mdBytes);
			var fileName = $"{SanitizeFileName(note.Title)}.md";
			var uploadResult = await _documentService.UploadDocumentAsync(ms, fileName, "text/markdown; charset=utf-8");

			if (!uploadResult.Success || uploadResult.Data == null)
				return BaseResponse<string>.Fail($"File upload failed: {uploadResult.Message}");

			// 4. Create document record
			var uploaded = uploadResult.Data;

			var createRequest = new CreateDocumentRequest
			{
				OwnerId = note.UserId,
				WorkSpaceId = note.WorkSpaceId,
				Title = fileName,
				ContentType = uploaded.ContentType ?? "text/markdown",
				Url = uploaded.Url
			};

			var createResult = await _documentService.CreateDocumentAsync(createRequest);
			if (!createResult.Success)
				return BaseResponse<string>.Fail($"Failed to create document record: {createResult.Message}");

			return BaseResponse<string>.Ok("Note exported to document successfully");
		}

		private static string SanitizeFileName(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return "untitled";
			var sanitized = Regex.Replace(name, @"[^\w\-. ]", "_");
			return sanitized.Trim();
		}

		public async Task<BaseResponse<string>> CreateNoteAsync(CreateNoteRequest request)
		{
			var validationResult = _createNoteValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors);
			}

			var note = _mapper.Map<Note>(request);
			note.CreatedAt = DateTime.UtcNow;
			await _noteRepository.AddAsync(note);
			var result = await _noteRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Note created successfully")
				: BaseResponse<string>.Fail("Failed to create Note");
		}
	}
}
