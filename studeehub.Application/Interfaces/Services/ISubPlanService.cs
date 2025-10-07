using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface ISubPlanService
	{
		public Task<BaseResponse<string>> CreateSubPlanAsync(CreateSubPlanRequest request);
		public Task<BaseResponse<string>> UpdateSubPlanAsync(Guid id, UpdateSubPlanRequest request);
		public Task<BaseResponse<string>> DeleteSubPlanAsync(Guid id);
	}
}
