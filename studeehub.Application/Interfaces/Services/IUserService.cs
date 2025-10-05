using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface IUserService
	{
		public Task<bool> IsUserExistAsync(Guid userId);
		public Task<User?> GetUserByIdAsync(Guid userId);

	}
}
