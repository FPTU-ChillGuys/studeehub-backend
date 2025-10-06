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
		public async Task SendScheduleReminder(Guid userId, object scheduleInfo)
		{
			await Clients.User(userId.ToString())
				.SendAsync("ScheduleReminder", scheduleInfo);
		}
		public async Task SendCheckinReminder(Guid userId, object scheduleInfo)
		{
			await Clients.User(userId.ToString())
				.SendAsync("ScheduleCheckinReminder", scheduleInfo);
		}
	}
}
