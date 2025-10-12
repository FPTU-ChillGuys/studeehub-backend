using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IPomodoroSessionRepository : IGenericRepository<PomodoroSession>
	{
		public Task<PomodoroSession?> GetLatestByUserAsync(Guid userId);
	}
}
