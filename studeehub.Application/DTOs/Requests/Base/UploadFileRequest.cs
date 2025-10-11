using Microsoft.AspNetCore.Http;

namespace studeehub.Application.DTOs.Requests.Base
{
	public class UploadFileRequest
	{
		// Support multiple files in a multipart/form-data request.
		// Model binding will populate this from multiple file inputs named "files" (or multiple files in a single input).
		public IList<IFormFile> Files { get; set; } = new List<IFormFile>();
	}
}
