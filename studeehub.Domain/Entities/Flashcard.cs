namespace studeehub.Domain.Entities
{
	public class Flashcard
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid WorkSpaceId { get; set; }
		public Guid UserId { get; set; }

		public string Question { get; set; } = string.Empty;
		public string Answer { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? LastReviewedAt { get; set; }

		public virtual User User { get; set; } = null!;
		public virtual WorkSpace WorkSpace { get; set; } = null!;
	}
}
