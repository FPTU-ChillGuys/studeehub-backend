using MapsterMapper;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class PayTransactionService : IPayTransactionService
	{
		private readonly IGenericRepository<PaymentTransaction> _paymentTransactionRepository;
		private readonly IMapper _mapper;

		public PayTransactionService(
			IGenericRepository<PaymentTransaction> paymentTransactionRepository,
			IMapper mapper)
		{
			_paymentTransactionRepository = paymentTransactionRepository;
			_mapper = mapper;
		}

		public async Task<BaseResponse<List<GetPayTXNResponse>>> GetPayTransactionsBySubscriptionId(Guid subscriptionId)
		{
			var transactions = await _paymentTransactionRepository.GetAllAsync(
				p => p.SubscriptionId == subscriptionId,
				orderBy: q => q.OrderByDescending(p => p.CreatedAt)
			);
			if (transactions == null || !transactions.Any())
				return BaseResponse<List<GetPayTXNResponse>>.Fail("No payment transactions found for this subscription", ErrorType.NotFound);

			var response = _mapper.Map<List<GetPayTXNResponse>>(transactions);
			return BaseResponse<List<GetPayTXNResponse>>.Ok(response, "Payment transactions retrieved");
		}
	}
}
