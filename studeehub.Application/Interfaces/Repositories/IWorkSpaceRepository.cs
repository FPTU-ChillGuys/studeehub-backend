using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IWorkSpaceRepository : IGenericRepository<WorkSpace>
	{
		public Task<string> GenerateUniqueWorkspaceNameAsync(Guid userId);
	}
}
