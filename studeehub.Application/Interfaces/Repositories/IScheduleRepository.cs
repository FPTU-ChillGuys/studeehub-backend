using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IScheduleRepository : IGenericRepository<Schedule>
	{
		public Task<IEnumerable<Schedule>> GetUpcomingSchedulesToRemindAsync(DateTime now);
		public Task<IEnumerable<Schedule>> GetCheckinSchedulesToRemindAsync(DateTime now);
	}
}
