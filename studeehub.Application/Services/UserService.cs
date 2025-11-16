using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using System.Linq.Expressions;

namespace studeehub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly ISupabaseStorageService _supabaseStorageService;
		private readonly IValidator<UpdateUserRequest> _updateUserValidator;
		private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, IMapper mapper, IValidator<UpdateUserRequest> updateUserValidator, ISupabaseStorageService supabaseStorageService)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_updateUserValidator = updateUserValidator;
			_supabaseStorageService = supabaseStorageService;
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

			// Ensure profile picture URL has a valid signed token; regenerate if expired.
			if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
			{
				try
				{
					var validUrl = await _supabaseStorageService.EnsureSignedUrlAsync(user.ProfilePictureUrl, user.Id.ToString());
					// If we could generate a fresh URL, return it to client.
					if (!string.IsNullOrWhiteSpace(validUrl))
						response.ProfilePictureUrl = validUrl;
				}
				catch
				{
					return BaseResponse<GetUserResponse>.Ok(response, "User profile retrieved successfully, but failed to process profile picture URL");
				}
			}

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

			// If a file was uploaded, upload it to Supabase and set ProfilePictureUrl on the request
			if (request.File != null)
			{
				try
				{
					using var stream = request.File.OpenReadStream();
					var uploadedUrl = await _supabaseStorageService.UploadUserAvatarAsync(stream, userId.ToString(), request.File.FileName);
					if (uploadedUrl == null)
					{
						return BaseResponse<string>.Fail("Failed to upload profile picture", Domain.Enums.ErrorType.ServerError);
					}

					request.ProfilePictureUrl = uploadedUrl;
				}
				catch (Exception ex)
				{
					// Bubble up a friendly error to the client while preserving internal details in logs (if any)
					return BaseResponse<string>.Fail("Failed to upload profile picture: " + ex.Message, Domain.Enums.ErrorType.ServerError);
				}
			}

			// Map incoming fields (including ProfilePictureUrl if set) to the user entity
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
				filter = filter.AndAlso(EfCollationExtensions.CollateContains<User>(u => u.FullName, keyword));
			}

			if (!string.IsNullOrWhiteSpace(request.Address))
			{
				var keyword = request.Address.Trim();
				filter = filter.AndAlso(EfCollationExtensions.CollateContains<User>(u => u.Address, keyword));
			}

			if (!string.IsNullOrWhiteSpace(request.Email))
			{
				var keyword = request.Email.Trim();
				filter = filter.AndAlso(EfCollationExtensions.CollateContains<User>(u => u.Email, keyword));
			}

			if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
			{
				filter = filter.AndAlso(u => u.PhoneNumber != null && u.PhoneNumber.Contains(request.PhoneNumber));
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

		public async Task<BaseResponse<List<GetUserLookupResponse>>> GetUserLookupAsync(GetUserLookupRequest request)
		{
			// Base filter
			Expression<Func<User, bool>> filter = u => true;
			if (request.IsActiveOnly)
				filter = filter.AndAlso(u => u.IsActive);

			var users = (await _userRepository.GetAllAsync(filter: filter, asNoTracking: true)).ToList();

			if (!users.Any())
			{
				return BaseResponse<List<GetUserLookupResponse>>.Ok(new List<GetUserLookupResponse>());
			}

			if (!request.IncludeAdmins)
			{
				// Filter out users who are admins using repository helper
				var ids = users.Select(u => u.Id).ToList();
				var rolesMap = await _userRepository.GetUserRolesAsync(ids);
				users = users.Where(u => !(rolesMap.TryGetValue(u.Id, out var roles) && roles.Any(r => string.Equals(r, "ADMIN", StringComparison.OrdinalIgnoreCase)))).ToList();
			}

			var responses = users.Select(u => new GetUserLookupResponse { Id = u.Id, FullName = u.FullName, Email = u.Email ?? string.Empty }).ToList();
			return BaseResponse<List<GetUserLookupResponse>>.Ok(responses);
		}
	}
}
