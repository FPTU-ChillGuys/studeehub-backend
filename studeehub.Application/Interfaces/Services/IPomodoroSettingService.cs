using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PomodoroSetting;
using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface IPomodoroSettingService
	{
		public Task<BaseResponse<string>> UpdateAsync(Guid userId, UpdateSettingRequest request);
		public Task<PomodoroSetting> GetForUserAsync(Guid userId);
		public Task<BaseResponse<GetSettingResponse>> GetSettingByUserIdAsync(Guid userId);
	}
}
