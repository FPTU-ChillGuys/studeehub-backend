using Microsoft.AspNetCore.Http;

namespace studeehub.Application.DTOs.Requests.Base
{
	public class UploadFileRequest
	{
		public IFormFile File { get; set; }
	}
}
