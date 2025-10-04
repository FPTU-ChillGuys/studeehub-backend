using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface IStreakService
	{
		public Task<BaseResponse<string>> CreateStreakAsync(CreateStreakRequest request);
		public Task<IEnumerable<User>> GetUsersToRemindAsync();
		public Task<BaseResponse<string>> UpdateStreakAsync(Guid userId, UpdateStreakRequest request);
	}
}
