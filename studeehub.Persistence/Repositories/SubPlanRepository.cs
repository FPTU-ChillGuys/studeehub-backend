using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class SubPlanRepository : GenericRepository<SubscriptionPlan>, ISubPlanRepository
	{
		public SubPlanRepository(StudeeHubDBContext context) : base(context)
		{
		}
	}
}
