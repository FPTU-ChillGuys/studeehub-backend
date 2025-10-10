using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Document;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class DocumentService : IDocumentService
	{
		private readonly ISupabaseStorageService _supabaseStorageService;
		private readonly IGenericRepository<Document> _genericRepository;
		private readonly IValidator<CreateDocumentRequest> _createDocumentValidator;
		private readonly IValidator<UpdateDocumentRequest> _updateDocumentValidator;
		private readonly IMapper _mapper;
		private readonly ILogger<DocumentService> _logger;

		public DocumentService(ISupabaseStorageService supabaseStorageService, IGenericRepository<Document> genericRepository, IValidator<CreateDocumentRequest> createDocumentValidator, IMapper mapper, IValidator<UpdateDocumentRequest> updateDocumentValidator, ILogger<DocumentService> logger)
		{
			_supabaseStorageService = supabaseStorageService;
			_genericRepository = genericRepository;
			_createDocumentValidator = createDocumentValidator;
			_mapper = mapper;
			_updateDocumentValidator = updateDocumentValidator;
			_logger = logger;
		}

		public async Task<BaseResponse<string>> CreateDocumentAsync(CreateDocumentRequest request)
		{
			var validationResult = await _createDocumentValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var document = _mapper.Map<Document>(request);

			await _genericRepository.AddAsync(document);
			var result = await _genericRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Document created successfully")
				: BaseResponse<string>.Fail("Failed to create Document", Domain.Enums.ErrorType.ServerError);
		}

		public async Task<BaseResponse<UploadFileResponse>> UploadDocumentAsync(Stream fileStream, string fileName, string contentType)
		{
			var uploadedUrl = await _supabaseStorageService.UploadFileAsync(fileStream, fileName);

			if (uploadedUrl == null)
				return BaseResponse<UploadFileResponse>.Fail("File upload failed", Domain.Enums.ErrorType.ServerError);

			var response = new UploadFileResponse
			{
				FileName = fileName,
				ContentType = contentType,
				Url = uploadedUrl
			};

			return BaseResponse<UploadFileResponse>.Ok(response);
		}

		public async Task<BaseResponse<string>> UpdateDocumentAsync(Guid id, UpdateDocumentRequest request)
		{
			var validationResult = _updateDocumentValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var document = await _genericRepository.GetByConditionAsync(d => d.Id == id);

			if (document == null)
			{
				return BaseResponse<string>.Fail("Document not found", Domain.Enums.ErrorType.NotFound);
			}

			var updatedDocument = _mapper.Map(request, document);
			_genericRepository.Update(updatedDocument);
			var result = await _genericRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Document updated successfully")
				: BaseResponse<string>.Fail("Failed to update Document", Domain.Enums.ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteDocumentAsync(Guid id)
		{
			var document = await _genericRepository.GetByConditionAsync(d => d.Id == id);
			if (document == null)
				return BaseResponse<string>.Fail("Document not found", ErrorType.NotFound);

			try
			{
				// If there is a FilePath, attempt to delete it from storage. Do not fail the entire operation
				// if extraction or deletion of the file path fails — continue and still attempt DB removal.
				if (!string.IsNullOrWhiteSpace(document.FilePath))
				{
					try
					{
						var filePath = await _supabaseStorageService.ExtractFilePathFromUrl(document.FilePath);

						// If delete fails, do not throw; log/continue. We don't have a logger here so observe the boolean.
						var deleted = await _supabaseStorageService.DeleteFileAsync(filePath);
						// If you want to fail the whole operation when storage delete fails, change behavior here.
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error deleting file from storage for Document ID {DocumentId}", id);
						// Swallow per-file-delete errors to avoid leaving the DB entry undeleted.
						// Consider logging this exception with a real ILogger in production.
					}
				}

				// Delete DB entry
				_genericRepository.Remove(document);
				var result = await _genericRepository.SaveChangesAsync();

				return result
					? BaseResponse<string>.Ok("Document deleted successfully")
					: BaseResponse<string>.Fail("Failed to delete document", ErrorType.ServerError);
			}
			catch (Exception ex)
			{
				// Handle partial failures (file deleted but db not saved, etc.)
				return BaseResponse<string>.Fail($"Error deleting document: {ex.Message}", ErrorType.ServerError);
			}
		}

		public async Task<BaseResponse<List<GetDocumentResponse>>> GetDocumentsByWorkSpaceIdAsync(Guid workSpaceId)
		{
			var documents = await _genericRepository.GetAllAsync(d => d.WorkSpaceId == workSpaceId);
			if (documents == null || !documents.Any())
			{
				return BaseResponse<List<GetDocumentResponse>>.Fail("No documents found for the specified WorkSpaceId", ErrorType.NotFound);
			}
			var response = _mapper.Map<List<GetDocumentResponse>>(documents);
			foreach (var doc in documents)
			{
				if (!string.IsNullOrWhiteSpace(doc.FilePath))
				{
					try
					{
						var filePath = await _supabaseStorageService.ExtractFilePathFromUrl(doc.FilePath);
						var signedUrl = await _supabaseStorageService.GenerateSignedUrlAsync(filePath);
						var respDoc = response.FirstOrDefault(r => r.Id == doc.Id);
						if (respDoc != null)
						{
							respDoc.FilePath = signedUrl;
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error generating signed URL for Document ID {DocumentId}", doc.Id);
						// If signed URL generation fails, we can still return the document without it.
						var respDoc = response.FirstOrDefault(r => r.Id == doc.Id);
						if (respDoc != null)
						{
							respDoc.FilePath = null;
						}
					}
				}
			}
			return BaseResponse<List<GetDocumentResponse>>.Ok(response);
		}

		public async Task<BaseResponse<GetDocumentResponse>> GetDocumentByIdAsync(Guid id)
		{
			var document = await _genericRepository.GetByConditionAsync(d => d.Id == id);
			if (document == null)
				return BaseResponse<GetDocumentResponse>.Fail("Document not found", ErrorType.NotFound);
			var response = _mapper.Map<GetDocumentResponse>(document);
			if (!string.IsNullOrWhiteSpace(document.FilePath))
			{
				try
				{
					var filePath = await _supabaseStorageService.ExtractFilePathFromUrl(document.FilePath);
					var signedUrl = await _supabaseStorageService.GenerateSignedUrlAsync(filePath);
					response.FilePath = signedUrl;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error generating signed URL for Document ID {DocumentId}", id);
					// If signed URL generation fails, we can still return the document without it.
					response.FilePath = null;
				}
			}
			return BaseResponse<GetDocumentResponse>.Ok(response);
		}
	}
}
