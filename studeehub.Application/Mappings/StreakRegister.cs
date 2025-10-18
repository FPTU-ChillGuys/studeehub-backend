using Mapster;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Responses.Streak;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class StreakRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateStreakRequest, Streak>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.CurrentCount, src => 1)
				.Map(dest => dest.LongestCount, src => 1)
				.Map(dest => dest.Type, src => src.Type)
				.Map(dest => dest.IsActive, src => true)
				.Map(dest => dest.LastUpdated, src => DateTime.UtcNow);

			config.NewConfig<UpdateStreakRequest, Streak>()
				.Map(dest => dest.LastUpdated, src => DateTime.UtcNow);

			config.NewConfig<Streak, GetStreakResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Type, src => src.Type)
				.Map(dest => dest.CurrentCount, src => src.CurrentCount)
				.Map(dest => dest.LongestCount, src => src.LongestCount)
				.Map(dest => dest.LastUpdated, src => src.LastUpdated)
				.Map(dest => dest.CreatedAt, src => src.CreatedAt);
		}
	}
}
