namespace studeehub.Domain.Entities
{
	public class Document
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }
		public Guid WorkSpaceId { get; set; }

		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
		public string FilePath { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public virtual User User { get; set; } = null!;
		public virtual WorkSpace WorkSpace { get; set; } = null!;
	}
}
