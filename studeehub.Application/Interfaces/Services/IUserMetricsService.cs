using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.User;

namespace studeehub.Application.Interfaces.Services
{
	public interface IUserMetricsService
	{
		public Task<BaseResponse<UserMetricsResponse>> GetAdminMetricsAsync(GetUserMetricsRequest request);
		public Task<BaseResponse<UserSelfMetricsResponse>> GetUserSelfMetricsAsync(Guid userId, GetUserSelfMetricsRequest request);
	}
}
