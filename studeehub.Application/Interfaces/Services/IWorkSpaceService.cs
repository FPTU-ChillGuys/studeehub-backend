using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface IWorkSpaceService
	{
		public Task<BaseResponse<string>> CreateWorkSpaceAsync(CreateWorkSpaceRequest requests);
	}
}
