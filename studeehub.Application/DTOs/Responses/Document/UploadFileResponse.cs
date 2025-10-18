namespace studeehub.Application.DTOs.Responses.Document
{
	public class UploadFileResponse
	{
		public string FileName { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
	}
}
