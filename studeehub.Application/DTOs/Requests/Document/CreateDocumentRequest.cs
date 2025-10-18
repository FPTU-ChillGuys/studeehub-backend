namespace studeehub.Application.DTOs.Requests.Document
{
	public class CreateDocumentRequest
	{
		public Guid OwnerId { get; set; }
		public Guid WorkSpaceId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty; // AI generated
		public string ContentType { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
	}
}
