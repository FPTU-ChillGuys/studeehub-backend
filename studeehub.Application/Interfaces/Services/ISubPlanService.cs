using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Requests.SubscriptionPlan;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.SubPlan;

namespace studeehub.Application.Interfaces.Services
{
	public interface ISubPlanService
	{
		public Task<BaseResponse<List<GetSubPlanResponse>>> GetAllSubPlansAsync();
		public Task<BaseResponse<List<GetSubPlanLookupResponse>>> GetSubPlanLookupAsync(GetSubPlanLookupRequest request);
		public Task<BaseResponse<GetSubPlanResponse>> GetSubPlanByIdAsync(Guid id);
		public Task<BaseResponse<string>> CreateSubPlanAsync(CreateSubPlanRequest request);
		public Task<BaseResponse<string>> UpdateSubPlanAsync(Guid id, UpdateSubPlanRequest request);
		public Task<BaseResponse<string>> DeleteSubPlanAsync(Guid id);
	}
}
