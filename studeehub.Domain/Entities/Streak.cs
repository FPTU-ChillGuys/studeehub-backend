using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Domain.Entities
{
    public class Streak
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public int CurrentCount { get; set; } = 0;
        public int LongestCount { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
