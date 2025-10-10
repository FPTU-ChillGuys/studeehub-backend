using MapsterMapper;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IGenericRepository<User> _userRepository;
		private readonly IMapper _mapper;

		public UserService(IGenericRepository<User> userRepository, IMapper mapper)
		{
			_userRepository = userRepository;
			_mapper = mapper;
		}

		public Task<bool> IsUserExistAsync(Guid userId)
		{
			return _userRepository.AnyAsync(u => u.Id == userId);
		}

		public Task<User?> GetUserByIdAsync(Guid userId)
		{
			return _userRepository.GetByConditionAsync(u => u.Id == userId);
		}

		public async Task<BaseResponse<GetUserResponse>> GetProfileByIdAsync(Guid userId)
		{
			var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (user == null)
			{
				return BaseResponse<GetUserResponse>.Fail("User not found", Domain.Enums.ErrorType.NotFound);

			}
			else
			{
				var response = _mapper.Map<GetUserResponse>(user);
				return BaseResponse<GetUserResponse>.Ok(response, "User profile retrieved successfully");
			}
		}
	}
}
