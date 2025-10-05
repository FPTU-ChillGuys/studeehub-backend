using Microsoft.AspNetCore.SignalR;
using studeehub.Application.DTOs.Requests.Achievement;

namespace studeehub.Infrastructure.Extensions
{
	public class NotificationHub : Hub
	{
		public async Task SendAchievementUnlocked(Guid userId, GetAchievemRequest achievement)
		{
			await Clients.User(userId.ToString())
				.SendAsync("AchievementUnlocked", achievement);
		}
	}
}
