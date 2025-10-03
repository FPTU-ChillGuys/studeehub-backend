using Mapster;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class DocumentRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateDocumentRequest, Document>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.UserId, src => src.OwnerId)
				.Map(dest => dest.WorkSpaceId, src => src.WorkSpaceId)
				.Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Type, src => src.ContentType)
				.Map(dest => dest.FilePath, src => src.Url);

			config.NewConfig<UpdateDocumentRequest, Document>()
				.Map(dest => dest.Title, src => src.Title);

		}
	}
}
