using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IValidator<UpdateUserRequest> _updateUserValidator;
		private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, IMapper mapper, IValidator<UpdateUserRequest> updateUserValidator)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_updateUserValidator = updateUserValidator;
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

			var response = _mapper.Map<GetUserResponse>(user);
			return BaseResponse<GetUserResponse>.Ok(response, "User profile retrieved successfully");
		}

		public async Task<BaseResponse<string>> UpdateProfileAsync(Guid userId, UpdateUserRequest request)
		{
			var validationResult = await _updateUserValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				return BaseResponse<string>.Fail("Validation errors: " + string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)), Domain.Enums.ErrorType.Validation);
			}
			var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (user == null)
			{
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);
			}
			_mapper.Map(request, user);
			_userRepository.Update(user);
			var result = await _userRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Profile updated successfully")
				: BaseResponse<string>.Fail("Failed to update profile", Domain.Enums.ErrorType.ServerError);
		}
	}
}
