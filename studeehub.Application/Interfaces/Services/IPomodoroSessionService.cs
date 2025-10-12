using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PomodoroSession;
using studeehub.Domain.Enums.Pomodoros;

namespace studeehub.Application.Interfaces.Services
{
	public interface IPomodoroSessionService
	{
		public Task<PagedResponse<GetSessionResponse>> GetSessionsAsync(Guid userId, GetSessionsRequest request);
		public Task<PagedResponse<GetSessionHistoryResponse>> GetSessionsAndStatsAsync(Guid userId, GetSessionsRequest request);
		public Task<BaseResponse<GetSessionResponse>> SkipSessionAsync(Guid userId);
		public Task<BaseResponse<GetSessionResponse>> CompleteSessionAsync(Guid userId);
		public Task<BaseResponse<GetSessionResponse>> StartSessionAsync(Guid userId, PomodoroType type);
	}
}
