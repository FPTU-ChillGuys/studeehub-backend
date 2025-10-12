using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		public UserRepository(StudeeHubDBContext context) : base(context)
		{
		}
	}
}
