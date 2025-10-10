using Mapster;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class UserRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<User, GetUserResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.FullName, src => src.FullName)
				.Map(dest => dest.Email, src => src.Email)
				.Map(dest => dest.UserName, src => src.UserName)
				.Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
				.Map(dest => dest.CreatedAt, src => src.CreatedAt)
				.Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
		}
	}
}
