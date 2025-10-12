using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class PomodoroSessionRepository : GenericRepository<PomodoroSession>, IPomodoroSessionRepository
	{
		public PomodoroSessionRepository(StudeeHubDBContext context) : base(context)
		{
		}

		public async Task<PomodoroSession?> GetLatestByUserAsync(Guid userId)
		{
			return await _context.PomodoroSessions
				.Where(ps => ps.UserId == userId)
				.OrderByDescending(ps => ps.End)
				.FirstOrDefaultAsync();
		}
	}
}
