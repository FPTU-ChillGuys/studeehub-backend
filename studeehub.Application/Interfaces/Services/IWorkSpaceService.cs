using studeehub.Application.DTOs.Requests.Workspace;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Workspace;

namespace studeehub.Application.Interfaces.Services
{
	public interface IWorkspaceService
	{
		public Task<BaseResponse<List<GetWorkspaceResponse>>> GetWorkspacesByUserIdAsync(Guid userId);
		public Task<BaseResponse<GetWorkspaceResponse>> GetWorkspaceByIdAsync(Guid id);
		public Task<BaseResponse<string>> CreateWorkspaceAsync(CreateWorkspaceRequest requests);
		public Task<BaseResponse<string>> UpdateWorkspaceAsync(Guid id, UpdateWorkspaceRequest requests);
		public Task<BaseResponse<string>> DeleteWorkspaceAsync(Guid id);
	}
}
