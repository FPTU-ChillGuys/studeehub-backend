using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Domain.Entities
{
    public class WorkSpace
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
