using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Note;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NotesController : ControllerBase
	{
		private readonly INoteService _noteService;

		public NotesController(INoteService noteService)
		{
			_noteService = noteService;
		}

		// GET /api/users/{userId}/notes
		[HttpGet("/api/users/{userId:Guid}/notes")]
		[ProducesResponseType(typeof(BaseResponse<List<GetNoteResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<List<GetNoteResponse>>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<List<GetNoteResponse>>> GetNotesByUserId([FromRoute] Guid userId)
			=> await _noteService.GetNotesByWorkSpaceIdAsync(userId);

		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<GetNoteResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<GetNoteResponse>), StatusCodes.Status404NotFound)]
		public async Task<BaseResponse<GetNoteResponse>> GetNoteById([FromRoute] Guid id)
			=> await _noteService.GetNoteByIdAsync(id);

		[HttpPost]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> CreateNote([FromBody] CreateNoteRequest request)
			=> await _noteService.CreateNoteAsync(request);

		[HttpPost("{id:Guid}/export")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> ExportNoteToDocument([FromRoute] Guid id)
			=> await _noteService.BecomeDocumentAsync(id);

		[HttpPut("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> UpdateNote([FromRoute] Guid id, [FromBody] UpdateNoteRequest request)
			=> await _noteService.UpdateNoteAsync(id, request);

		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
		public async Task<BaseResponse<string>> DeleteNote([FromRoute] Guid id)
			=> await _noteService.DeleteNoteAsync(id);
	}
}
