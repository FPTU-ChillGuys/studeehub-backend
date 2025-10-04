using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IGenericRepository<User> _userRepository;

		public UserService(IGenericRepository<User> userRepository)
		{
			_userRepository = userRepository;
		}

		public Task<bool> IsUserExistAsync(Guid userId)
		{
			return _userRepository.AnyAsync(u => u.Id == userId);
		}
	}
}
