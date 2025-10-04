using Mapster;
using studeehub.Application.DTOs.Requests.Streak;
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
				.Map(dest => dest.LastUpdated, src => DateTime.UtcNow);

			config.NewConfig<UpdateStreakRequest, Streak>()
				.Map(dest => dest.LastUpdated, src => DateTime.UtcNow);
		}
	}
}
