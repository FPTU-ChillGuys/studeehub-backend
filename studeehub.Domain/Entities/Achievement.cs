using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Domain.Entities
{
    public class Achievement
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
