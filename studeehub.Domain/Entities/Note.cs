namespace studeehub.Domain.Entities
{
	public class Note
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid WorkSpaceId { get; set; }

		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public virtual User User { get; set; } = null!;
		public virtual WorkSpace WorkSpace { get; set; } = null!;
	}
}
