using Microsoft.AspNetCore.SignalR;
using studeehub.Application.DTOs.Responses.Achievement;

namespace studeehub.Infrastructure.Extensions
{
	public class NotificationHub : Hub
	{
		// user achievement service
		public async Task SendAchievementUnlocked(Guid userId, GetAchievemResponse achievement)
		{
			await Clients.User(userId.ToString())
				.SendAsync("AchievementUnlocked", achievement);
		}

		// send remider job service
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
