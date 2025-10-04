using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IStreakRepository : IGenericRepository<Streak>
	{
		public Task<IEnumerable<User>> GetUsersToRemindAsync();
	}
}
