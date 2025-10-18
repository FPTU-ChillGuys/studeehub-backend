using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface IUserAchievementService
	{
		public Task CheckAndUnlockAsync(User user);
	}
}
