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

		[HttpGet("workspace/{workSpaceId:Guid}")]
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
		[ProducesResponseType(typeof(BaseResponse<UploadFileResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<UploadFileResponse>> Upload([FromForm] UploadFileRequest request)
		{
			var uploadedFile = request.File;
			if (uploadedFile == null || uploadedFile.Length == 0)
			{
				return BaseResponse<UploadFileResponse>.Fail("File is not null", ErrorType.Validation);
			}

			await using var stream = uploadedFile.OpenReadStream();

			return await _documentService.UploadDocumentAsync(
				stream,
				uploadedFile.FileName,
				uploadedFile.ContentType
			);
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
