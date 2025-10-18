namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISubscriptionJobService
	{
		public Task CheckPendingSubscriptionsAsync();
		public Task CheckExpiredSubscriptionsAsync();

	}
}
