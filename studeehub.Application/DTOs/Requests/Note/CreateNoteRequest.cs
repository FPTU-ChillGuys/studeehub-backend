namespace studeehub.Application.DTOs.Requests.Note
{
	public class CreateNoteRequest
	{
		public Guid OwnerId { get; set; }
		public Guid WorkSpaceId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
	}
}
