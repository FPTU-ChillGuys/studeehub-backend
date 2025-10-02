using Mapster;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class NoteRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateNoteRequest, Note>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.WorkSpaceId, src => src.WorkSpaceId)
				.Map(dest => dest.UserId, src => src.OwnerId)
				.Map(dest => dest.CreatedAt, src => DateTime.UtcNow);
		}
	}
}
