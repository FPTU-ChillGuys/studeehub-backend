using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface IAchievementService
	{
		public Task<BaseResponse<string>> CreateAchievementAsync(CreateAchievemRequest request);
		public Task<BaseResponse<string>> UpdateAchievementAsync(Guid id, UpdateAchievemRequest request);
	}
}
