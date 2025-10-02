using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Base;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.Interfaces.Services;

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

		[HttpPost("upload")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
		{
			var uploadedFile = request.File;
			if (uploadedFile == null || uploadedFile.Length == 0)
				return BadRequest("File is required.");

			await using var stream = uploadedFile.OpenReadStream();

			var result = await _documentService.UploadDocumentAsync(
				stream,
				uploadedFile.FileName,
				uploadedFile.ContentType
			);

			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
		{
			var result = await _documentService.CreateDocumentAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateDocument([FromBody] UpdateDocumentRequest request)
		{
			var result = await _documentService.UpdateDocumentAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
