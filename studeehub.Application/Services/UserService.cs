using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
