using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IWorkSpaceRepository : IGenericRepository<Workspace>
	{
		public Task<string> GenerateUniqueWorkspaceNameAsync(Guid userId);
	}
}
