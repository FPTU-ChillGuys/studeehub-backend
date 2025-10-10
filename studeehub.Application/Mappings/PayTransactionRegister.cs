using Mapster;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	internal class PayTransactionRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<PaymentTransaction, GetPayTXNResponse>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Amount, src => src.Amount)
				.Map(dest => dest.Currency, src => src.Currency)
				.Map(dest => dest.Status, src => src.Status)
				.Map(dest => dest.PaymentMethod, src => src.PaymentMethod)
				.Map(dest => dest.CreatedAt, src => src.CreatedAt)
				.Map(dest => dest.CompletedAt, src => src.CompletedAt);
		}
	}
}
