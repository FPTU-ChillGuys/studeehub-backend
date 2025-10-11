using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Base;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Document;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Enums;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		private readonly IDocumentService _documentService;

		public DocumentController(IDocumentService documentService)
		{
			_documentService = documentService;
		}

		[HttpGet("Workspace/{workSpaceId:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<List<GetDocumentResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetDocumentResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetDocumentResponse>>> GetDocumentsByWorkSpaceId([FromRoute] Guid workSpaceId)
			=> await _documentService.GetDocumentsByWorkSpaceIdAsync(workSpaceId);

		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetDocumentResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetDocumentResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetDocumentResponse>> GetDocumentById([FromRoute] Guid id)
			=> await _documentService.GetDocumentByIdAsync(id);

		[HttpPost("upload")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(typeof(BaseResponse<IList<UploadFileResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<IList<UploadFileResponse>>> Upload([FromForm] UploadFileRequest request)
		{
			if (request?.Files == null || request.Files.Count == 0)
			{
				return BaseResponse<IList<UploadFileResponse>>.Fail("No files provided", ErrorType.Validation);
			}

			var uploadedResponses = new List<UploadFileResponse>();

			foreach (var uploadedFile in request.Files)
			{
				if (uploadedFile == null || uploadedFile.Length == 0)
					continue; // skip empty entries

				await using var stream = uploadedFile.OpenReadStream();

				// Reuse existing single-file UploadDocumentAsync for robustness and to ensure any
				// side-effects / validations done there remain consistent.
				var singleResponse = await _documentService.UploadDocumentAsync(
					stream,
					uploadedFile.FileName,
					uploadedFile.ContentType
				);

				if (singleResponse == null || !singleResponse.Success)
				{
					// Decide policy: fail-fast if any file upload fails (current behavior).
					// Alternatively, collect errors per-file and return partial successes.
					return BaseResponse<IList<UploadFileResponse>>.Fail("One or more files failed to upload", ErrorType.ServerError);
				}

				uploadedResponses.Add(singleResponse.Data);
			}

			return BaseResponse<IList<UploadFileResponse>>.Ok(uploadedResponses);
		}

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateDocument([FromBody] CreateDocumentRequest request)
			=> await _documentService.CreateDocumentAsync(request);

		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateDocument([FromRoute] Guid id, [FromBody] UpdateDocumentRequest request)
			=> await _documentService.UpdateDocumentAsync(id, request);

		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteDocument([FromRoute] Guid id)
			=> await _documentService.DeleteDocumentAsync(id);
	}
}
