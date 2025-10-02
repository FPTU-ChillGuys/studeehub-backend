using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool IsActive { get; set; } = true;

        public virtual ICollection<WorkSpace> WorkSpaces { get; set; } = new List<WorkSpace>();
        public virtual ICollection<PomodoroSession> PomodoroSessions { get; set; } = new List<PomodoroSession>();
        public virtual ICollection<Streak> Streaks { get; set; } = new List<Streak>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
