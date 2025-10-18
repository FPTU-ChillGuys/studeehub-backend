using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class PayTransactionRepository : GenericRepository<PaymentTransaction>, IPayTransactionRepository
	{
		public PayTransactionRepository(StudeeHubDBContext context) : base(context)
		{
		}
	}
}
