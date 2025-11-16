using Mapster;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class SubscriptionRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<Subscription, GetUserSubscriptionResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.SubscriptionPlanId, src => src.SubscriptionPlanId)
				.Map(dest => dest.StartDate, src => src.StartDate)
				.Map(dest => dest.EndDate, src => src.EndDate)
				.Map(dest => dest.Status, src => src.Status)
				.Map(dest => dest.SubscriptionPlan, src => src.SubscriptionPlan);

			config.NewConfig<Subscription, GetSubscriptionResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.SubscriptionPlanId, src => src.SubscriptionPlanId)
				.Map(dest => dest.StartDate, src => src.StartDate)
				.Map(dest => dest.EndDate, src => src.EndDate)
				.Map(dest => dest.Status, src => src.Status)
				.Map(dest => dest.SubscriptionPlan, src => src.SubscriptionPlan)
				.Map(dest => dest.User, src => src.User);
		}
	}
}
