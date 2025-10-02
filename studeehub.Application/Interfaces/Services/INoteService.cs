using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface INoteService
	{
		public Task<BaseResponse<string>> CreateNoteAsync(CreateNoteRequest request);
		public Task<BaseResponse<string>> UpdateNoteAsync(UpdateNoteRequest request);
		public Task<BaseResponse<string>> BecomeDocumentAsync(Guid noteId);
	}
}
