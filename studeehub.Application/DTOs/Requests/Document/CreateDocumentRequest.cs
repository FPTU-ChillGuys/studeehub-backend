namespace studeehub.Application.DTOs.Requests.Document
{
	public class CreateDocumentRequest
	{
		public Guid OwnerId { get; set; }
		public Guid WorkspaceId { get; set; }
		public string Title { get; set; } = string.Empty; // AI generated title
		public string ContentType { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
	}
}
