using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class StreakRepository : GenericRepository<Streak>, IStreakRepository
	{
		public StreakRepository(StudeeHubDBContext context) : base(context) { }

		public async Task<IEnumerable<User>> GetUsersToRemindAsync()
		{
			var today = DateTime.UtcNow.Date;

			// return active users who have at least one streak not updated since before today
			return await _context.Users
				.AsNoTracking()
				.Where(u => u.IsActive && u.Streaks.Any(s => s.LastUpdated < today))
				.ToListAsync();
		}
	}
}
