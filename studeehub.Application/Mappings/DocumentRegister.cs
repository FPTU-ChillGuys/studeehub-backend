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
				.Map(dest => dest.WorkSpaceId, src => src.WorkspaceId)
				.Map(dest => dest.FilePath, src => src.Url);
		}
	}
}
