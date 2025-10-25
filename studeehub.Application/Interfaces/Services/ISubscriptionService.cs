using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface ISubscriptionService
	{
		public Task<BaseResponse<GetSubscriptionResponse>> GetActiveSubscriptionByUserIdAsync(Guid userId);
		public Task<BaseResponse<List<GetSubscriptionResponse>>> GetSubscriptionsByUserIdAsync(Guid userId);
		public Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration);
		public Task Update(Subscription subscription);
		public Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
	}
}
