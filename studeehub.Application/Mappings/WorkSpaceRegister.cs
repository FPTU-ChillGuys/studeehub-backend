using Mapster;
using studeehub.Application.DTOs.Requests.Workspace;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class WorkSpaceRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateWorkspaceRequest, Workspace>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.UserId, src => src.OwnerId)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

			config.NewConfig<UpdateWorkspaceRequest, Workspace>()
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
		}
	}
}
