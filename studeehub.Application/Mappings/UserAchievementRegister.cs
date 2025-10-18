using Mapster;
using studeehub.Application.DTOs.Requests.UserAchievem;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class UserAchievementRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<UnlockAchivemRequest, UserAchievement>()
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.AchievementId, src => src.AchievementId)
				.Map(dest => dest.UnlockedAt, src => DateTime.UtcNow)
				.Map(dest => dest.IsClaimed, src => true);
		}
	}
}
