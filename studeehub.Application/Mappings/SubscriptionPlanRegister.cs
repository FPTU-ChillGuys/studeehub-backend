using Mapster;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class SubscriptionPlanRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateSubPlanRequest, SubscriptionPlan>()
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.Currency, src => src.Currency)
				.Map(dest => dest.DurationInDays, src => src.DurationInDays)
				.Map(dest => dest.IsDeleted, src => false)
				.Map(dest => dest.MaxDocuments, src => src.MaxDocuments)
				.Map(dest => dest.MaxStorageMB, src => src.MaxStorageMB)
				.Map(dest => dest.HasAIAnalysis, src => src.HasAIAnalysis);

			config.NewConfig<UpdateSubPlanRequest, SubscriptionPlan>()
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.Currency, src => src.Currency)
				.Map(dest => dest.DurationInDays, src => src.DurationInDays)
				.Map(dest => dest.IsActive, src => src.IsActive) // ensure IsActive is mapped on update
				.Map(dest => dest.MaxDocuments, src => src.MaxDocuments)
				.Map(dest => dest.MaxStorageMB, src => src.MaxStorageMB)
				.Map(dest => dest.HasAIAnalysis, src => src.HasAIAnalysis);
		}
	}
}
