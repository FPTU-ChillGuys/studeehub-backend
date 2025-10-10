namespace studeehub.Application.DTOs.Responses.Workspace
{
	public class GetWorkspaceResponse
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Collections to return documents, notes and flashcards
		public List<DocumentResponse>? Documents { get; set; }
		public List<NoteResponse>? Notes { get; set; }
		public List<FlashcardResponse>? Flashcards { get; set; }
	}

	// Simple DTOs for related entities. Keep them minimal and safe to expose.
	public class DocumentResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid WorkSpaceId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? Type { get; set; }
		public string? FilePath { get; set; }
	}

	public class NoteResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid WorkSpaceId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Content { get; set; }
	}

	public class FlashcardResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid WorkSpaceId { get; set; }
		public string Question { get; set; } = string.Empty;
		public string Answer { get; set; } = string.Empty;
	}
}
