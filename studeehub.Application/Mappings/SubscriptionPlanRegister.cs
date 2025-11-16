using Mapster;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.SubPlan;
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
				.Map(dest => dest.IsActive, src => true)
				.Map(dest => dest.DiscountPercentage, src => src.DiscountPercentage)
				.Map(dest => dest.DocumentUploadLimitPerDay, src => src.DocumentUploadLimitPerDay)
				.Map(dest => dest.MaxStorageMB, src => src.MaxStorageMB)
				.Map(dest => dest.AIQueriesPerDay, src => src.AIQueriesPerDay)
				.Map(dest => dest.FlashcardCreationLimitPerDay, src => src.FlashcardCreationLimitPerDay)
				.Map(dest => dest.HasAIAnalysis, src => src.HasAIAnalysis);

			config.NewConfig<UpdateSubPlanRequest, SubscriptionPlan>()
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.Currency, src => src.Currency)
				.Map(dest => dest.DurationInDays, src => src.DurationInDays)
				.Map(dest => dest.IsActive, src => src.IsActive)
				.Map(dest => dest.DiscountPercentage, src => src.DiscountPercentage)
				.Map(dest => dest.DocumentUploadLimitPerDay, src => src.DocumentUploadLimitPerDay)
				.Map(dest => dest.MaxStorageMB, src => src.MaxStorageMB)
				.Map(dest => dest.AIQueriesPerDay, src => src.AIQueriesPerDay)
				.Map(dest => dest.FlashcardCreationLimitPerDay, src => src.FlashcardCreationLimitPerDay)
				.Map(dest => dest.HasAIAnalysis, src => src.HasAIAnalysis);

			config.NewConfig<SubscriptionPlan, GetSubPlanResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.Currency, src => src.Currency)
				.Map(dest => dest.DurationInDays, src => src.DurationInDays)
				.Map(dest => dest.DiscountPercentage, src => src.DiscountPercentage)
				.Map(dest => dest.DocumentUploadLimitPerDay, src => src.DocumentUploadLimitPerDay)
				.Map(dest => dest.MaxStorageMB, src => src.MaxStorageMB)
				.Map(dest => dest.AIQueriesPerDay, src => src.AIQueriesPerDay)
				.Map(dest => dest.FlashcardCreationLimitPerDay, src => src.FlashcardCreationLimitPerDay)
				.Map(dest => dest.HasAIAnalysis, src => src.HasAIAnalysis);

			config.NewConfig<SubscriptionPlan, GetSubPlanLookupResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Code, src => src.Code);
		}
	}
}
