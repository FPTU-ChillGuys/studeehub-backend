namespace studeehub.Application.DTOs.Responses.Note
{
	public class GetNoteResponse
	{
		public Guid Id { get; set; }

		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
