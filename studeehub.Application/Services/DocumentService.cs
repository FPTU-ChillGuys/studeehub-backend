using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Document;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class DocumentService : IDocumentService
	{
		private readonly ISupabaseStorageService _supabaseStorageService;
		private readonly IGenericRepository<Document> _genericRepository;
		private readonly IValidator<CreateDocumentRequest> _createDocumentValidator;
		private readonly IValidator<UpdateDocumentRequest> _updateDocumentValidator;
        private readonly IMapper _mapper;

		public DocumentService(ISupabaseStorageService supabaseStorageService, IGenericRepository<Document> genericRepository, IValidator<CreateDocumentRequest> createDocumentValidator, IMapper mapper, IValidator<UpdateDocumentRequest> updateDocumentValidator)
		{
			_supabaseStorageService = supabaseStorageService;
			_genericRepository = genericRepository;
			_createDocumentValidator = createDocumentValidator;
			_mapper = mapper;
			_updateDocumentValidator = updateDocumentValidator;
		}

		public async Task<BaseResponse<string>> CreateDocumentAsync(CreateDocumentRequest request)
		{
			var validationResult = await _createDocumentValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors);
			}

			var document = _mapper.Map<Document>(request);

			await _genericRepository.AddAsync(document);
			var result = await _genericRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Document created successfully")
				: BaseResponse<string>.Fail("Failed to create Document");
		}

		public async Task<BaseResponse<UploadFileResponse>> UploadDocumentAsync(Stream fileStream, string fileName, string contentType)
		{
			var uploadedUrl = await _supabaseStorageService.UploadFileAsync(fileStream, fileName);

			if (uploadedUrl == null)
				return BaseResponse<UploadFileResponse>.Fail("File upload failed");

			var response = new UploadFileResponse
			{
				FileName = fileName,
				ContentType = contentType,
				Url = uploadedUrl
			};

			return BaseResponse<UploadFileResponse>.Ok(response);
		}

		public async Task<BaseResponse<string>> UpdateDocumentAsync(UpdateDocumentRequest request)
		{
			var validationResult = _updateDocumentValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors);
            }

			var document = await _genericRepository.GetByIdAsync(d => d.Id == request.Id);

			if (document == null)
			{
				return BaseResponse<string>.Fail("Document not found");
            }

			var updatedDocument = _mapper.Map(request, document);
			_genericRepository.Update(updatedDocument);
			var result = await _genericRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Document updated successfully")
				: BaseResponse<string>.Fail("Failed to update Document");
        }
	}
}
