namespace studeehub.Application.DTOs.Responses.User
{
	public class UserSelfMetricsResponse
	{
		// --- Basic Info ---
		public Guid UserId { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string? SubscriptionName { get; set; }
		public DateTime JoinedAt { get; set; }

		// --- Pomodoro Stats ---
		public int TotalSessions { get; set; }
		public double TotalFocusHours { get; set; }
		public double AvgFocusPerDay { get; set; }
		public List<DailyFocusSummary> FocusChart { get; set; } = new();

		// --- Schedules ---
		public int TotalSchedules { get; set; }
		public int CompletedSchedules { get; set; }
		public double CompletionRate { get; set; }

		// --- Streaks ---
		public int CurrentStreak { get; set; }
		public int LongestStreak { get; set; }

		// --- Achievements ---
		public int UnlockedAchievements { get; set; }
		public List<string> RecentAchievements { get; set; } = new();

		// --- Engagement ---
		public int ActiveDays { get; set; }
		public double EngagementRatePercent { get; set; } // activeDays / total period days * 100
	}

	public class DailyFocusSummary
	{
		public DateTime Date { get; set; }
		public double FocusHours { get; set; }
	}
}
