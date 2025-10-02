using Microsoft.AspNetCore.Mvc;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.Interfaces.Services;

namespace studeehub.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NoteController : ControllerBase
	{
		private readonly INoteService _noteService;

		public NoteController(INoteService noteService)
		{
			_noteService = noteService;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
		{
			var result = await _noteService.CreateNoteAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPost("{noteId:guid}/export")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ExportNoteToDocument([FromRoute] Guid noteId)
		{
			var result = await _noteService.BecomeDocumentAsync(noteId);
			return result.Success ? Ok(result) : BadRequest(result);
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteRequest request)
		{
			var result = await _noteService.UpdateNoteAsync(request);
			return result.Success ? Ok(result) : BadRequest(result);
		}
	}
}
