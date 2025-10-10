using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Streak;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Streaks;

namespace studeehub.Application.Interfaces.Services
{
	public interface IStreakService
	{
		public Task<BaseResponse<List<GetStreakResponse>>> GetStreakByUserIdAsync(Guid userId, StreakType? type);
		public Task<BaseResponse<GetStreakResponse>> GetStreakByIdAsync(Guid id);
		public Task<BaseResponse<string>> CreateStreakAsync(CreateStreakRequest request);
		public Task<IEnumerable<User>> GetUsersToRemindAsync();
		public Task<BaseResponse<string>> UpdateStreakAsync(Guid userId, UpdateStreakRequest request);
	}
}
