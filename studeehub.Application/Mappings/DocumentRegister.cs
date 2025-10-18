using Mapster;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Responses.Document;
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
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Type, src => src.ContentType)
				.Map(dest => dest.FilePath, src => src.Url);

			config.NewConfig<UpdateDocumentRequest, Document>()
				.Map(dest => dest.Name, src => src.Name);

			// Map Document entity to GetDocumentResponse DTO.
			// This keeps mapping logic in one place and avoids accidental projection issues elsewhere.
			config.NewConfig<Document, GetDocumentResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Type, src => src.Type)
				.Map(dest => dest.FilePath, src => src.FilePath)
				.Map(dest => dest.CreatedAt, src => src.CreatedAt)
				.Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
		}
	}
}
