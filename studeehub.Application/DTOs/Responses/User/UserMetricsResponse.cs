namespace studeehub.Application.DTOs.Responses.User
{
	public class UserMetricsResponse
	{
		// --- Overview ---
		public int TotalUsers { get; set; }
		public int ActiveUsers { get; set; }
		public int InactiveUsers { get; set; }

		// --- Growth ---
		public int NewUsers { get; set; }
		public double GrowthRatePercent { get; set; }

		// --- Subscriptions ---
		public double PayingUserRatio { get; set; }
		public Dictionary<string, int> UsersBySubscription { get; set; } = new();

		// --- Productivity stats ---
		public double AvgSessionsPerUser { get; set; }
		public int TotalSchedules { get; set; }
		public double AvgSchedulesPerUser { get; set; }

		// --- Streaks & Achievements ---
		public int TotalStreaks { get; set; }
		public int ActiveStreaks { get; set; }
		public double AvgStreakLength { get; set; }
		public int TotalAchievementsUnlocked { get; set; }
		public Dictionary<string, int> TopAchievements { get; set; } = new();

		// --- Retention ---
		public double Retention7DayPercent { get; set; }
		public double Retention30DayPercent { get; set; }

		// --- Trend & Chart ---
		public List<MonthlyUserCount> MonthlyGrowth { get; set; } = new();
		public UserTrendSummary Trend { get; set; } = new();
	}

	public class UserTrendSummary
	{
		public DateTime CurrentStart { get; set; }
		public DateTime CurrentEnd { get; set; }
		public DateTime PrevStart { get; set; }
		public DateTime PrevEnd { get; set; }
		public int CurrentNewUsers { get; set; }
		public int PrevNewUsers { get; set; }
		public double GrowthRatePercent { get; set; }
	}

	public class MonthlyUserCount
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public int Count { get; set; }
		public int? Week { get; set; }
	}
}
