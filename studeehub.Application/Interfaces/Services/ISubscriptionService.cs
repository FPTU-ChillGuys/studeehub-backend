using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Subscription;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Subscriptions;

namespace studeehub.Application.Interfaces.Services
{
	public interface ISubscriptionService
	{
		public Task<PagedResponse<GetSubscriptionResponse>> GetAllSubscriptionsAsync(GetPagedAndSortedSubscriptionsRequest request);
		public Task<BaseResponse<GetUserSubscriptionResponse>> GetActiveSubscriptionByUserIdAsync(Guid userId);
		public Task<BaseResponse<List<GetUserSubscriptionResponse>>> GetSubscriptionsByUserIdAsync(Guid userId);
		public Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration);
		public Task Update(Subscription subscription);
		public Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
		public Task<BaseResponse<string>> CreateSubscriptionAsync(CreateSubscriptionRequest request);
		public Task<BaseResponse<string>> UpdateSubscriptionStatusAsync(Guid subscriptionId, SubscriptionStatus status);
	}
}
