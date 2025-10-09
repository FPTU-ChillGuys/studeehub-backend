using Mapster;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class AchievementRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateAchievemRequest, Achievement>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.ConditionType, src => src.ConditionType)
				.Map(dest => dest.ConditionValue, src => src.ConditionValue)
				.Map(dest => dest.RewardType, src => src.RewardType)
				.Map(dest => dest.RewardValue, src => src.RewardValue)
				// IsActive default is true on entity; no need to map explicitly for create.
				;

			config.NewConfig<UpdateAchievemRequest, Achievement>()
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.ConditionType, src => src.ConditionType)
				.Map(dest => dest.ConditionValue, src => src.ConditionValue)
				.Map(dest => dest.RewardType, src => src.RewardType)
				.Map(dest => dest.RewardValue, src => src.RewardValue)
				.Map(dest => dest.IsActive, src => src.IsActive);

			config.NewConfig<Achievement, GetAchievemRequest>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.ConditionType, src => src.ConditionType)
				.Map(dest => dest.ConditionValue, src => src.ConditionValue)
				.Map(dest => dest.RewardType, src => src.RewardType)
				.Map(dest => dest.RewardValue, src => src.RewardValue);
		}
	}
}
