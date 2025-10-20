using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using System.Linq.Expressions;

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

		public async Task<PagedResponse<GetUserResponse>> GetUsersAsync(GetPagedAndSortedUsersRequest request)
		{
			// Start with a no-op predicate so we can safely compose with AndAlso extensions
			Expression<Func<User, bool>> filter = u => true;

			if (!string.IsNullOrWhiteSpace(request.FullName))
			{
				var keyword = request.FullName.Trim();

				// Compose a predicate that matches FullName or UserName using either Vietnamese or Latin1 collations.
				filter = filter.AndAlso(u =>
					u.FullName != null &&
					(
						EF.Functions.Collate(u.FullName, "Vietnamese_CI_AI").Contains(keyword) ||
						EF.Functions.Collate(u.FullName, "Latin1_General_CI_AI").Contains(keyword)
					)
				);
			}

			if (!string.IsNullOrWhiteSpace(request.Address))
			{
				filter = filter.AndAlso(u => u.Address != null && u.Address.Contains(request.Address));
			}

			if (request.IsActive.HasValue)
			{
				filter = filter.AndAlso(u => u.IsActive == request.IsActive.Value);
			}

			if (request.CreatedFrom.HasValue)
			{
				// DateOnly -> DateTime at start of day
				var createdFrom = request.CreatedFrom.Value.ToDateTime(new TimeOnly(0, 0));
				filter = filter.AndAlso(u => u.CreatedAt >= createdFrom);
			}

			if (request.CreatedTo.HasValue)
			{
				// include the entire 'to' date by using end of day
				var createdTo = request.CreatedTo.Value.ToDateTime(TimeOnly.MaxValue);
				filter = filter.AndAlso(u => u.CreatedAt <= createdTo);
			}

			var (users, totalCount) = await _userRepository.GetPagedAsync(
				filter: filter,
				include: null,
				orderBy: u => u.ApplySorting(request.SortBy, request.SortDescending),
				pageNumber: request.PageNumber,
				pageSize: request.PageSize,
				asNoTracking: true);

			if (users == null || !users.Any())
			{
				return PagedResponse<GetUserResponse>.Ok(new List<GetUserResponse>(), 0, request.PageNumber, request.PageSize);
			}

			var responses = _mapper.Map<List<GetUserResponse>>(users);
			return PagedResponse<GetUserResponse>.Ok(responses, totalCount, request.PageNumber, request.PageSize);
		}

		public async Task<BaseResponse<string>> UpdateUserStatus(Guid userId, bool isActive)
		{
			var user = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (user == null)
			{
				return BaseResponse<string>.Fail("User not found", Domain.Enums.ErrorType.NotFound);
			}
			user.IsActive = isActive;
			_userRepository.Update(user);
			var result = await _userRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("User status updated successfully")
				: BaseResponse<string>.Fail("Failed to update user status", Domain.Enums.ErrorType.ServerError);
		}
	}
}
