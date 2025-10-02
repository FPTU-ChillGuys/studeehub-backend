using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Document;

namespace studeehub.Application.Interfaces.Services
{
	public interface IDocumentService
	{
		public Task<BaseResponse<UploadFileResponse>> UploadDocumentAsync(Stream fileStream, string fileName, string contentType);

		public Task<BaseResponse<string>> CreateDocumentAsync(CreateDocumentRequest request);
	}
}
