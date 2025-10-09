using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Achievement;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface IAchievementService
	{
		public Task<PagedResponse<GetAchievemResponse>> GetPagedAchievementsAsync(GetAchievemsRequest request);
		public Task<BaseResponse<GetAchievemResponse>> GetAchievementByIdAsync(Guid id);
		public Task<BaseResponse<string>> CreateAchievementAsync(CreateAchievemRequest request);
		public Task<BaseResponse<string>> UpdateAchievementAsync(Guid id, UpdateAchievemRequest request);
		public Task<BaseResponse<string>> DeleteAchievementAsync(Guid id);
	}
}
