using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Responses.Base;

namespace studeehub.Application.Interfaces.Services
{
	public interface INoteService
	{
		public Task<BaseResponse<string>> CreateNoteAsync(CreateNoteRequest request);
		public Task<BaseResponse<string>> UpdateNoteAsync(Guid noteId, UpdateNoteRequest request);
		public Task<BaseResponse<string>> BecomeDocumentAsync(Guid noteId);
		public Task<BaseResponse<string>> DeleteNoteAsync(Guid noteId);
	}
}
