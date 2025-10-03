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
				.Map(dest => dest.Title, src => "New Note")
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

			config.NewConfig<UpdateNoteRequest, Note>()
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
		}
	}
}
