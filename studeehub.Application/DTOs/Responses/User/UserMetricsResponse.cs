namespace studeehub.Application.DTOs.Responses.User
{
	public class UserMetricsResponse
	{
		public int TotalUsers { get; set; }
		public int ActiveUsers { get; set; }
		public int InactiveUsers { get; set; }

		public int NewUsersCount { get; set; }
		public double GrowthRatePercent { get; set; }

		public double PayingUserRatio { get; set; }

		public Dictionary<string, int> UsersBySubscription { get; set; } = new();

		public double AverageSessionsPerUser { get; set; }

		public List<MonthlyUserCount> MonthlyUserGrowth { get; set; } = new();

		public int TotalSchedules { get; set; }
		public double AverageSchedulesPerUser { get; set; }

		public int TotalStreaks { get; set; }
		public int ActiveStreaks { get; set; }
		public double AverageStreakLength { get; set; }

		public int TotalAchievementsUnlocked { get; set; }
		public Dictionary<string, int> TopAchievements { get; set; } = new();

		public double Retention7DayPercent { get; set; }
		public double Retention30DayPercent { get; set; }

		public UserTrendSummary TrendSummary { get; set; } = new();
	}

	public class UserTrendSummary
	{
		public DateTime CurrentPeriodStart { get; set; }
		public DateTime CurrentPeriodEnd { get; set; }
		public DateTime PreviousPeriodStart { get; set; }
		public DateTime PreviousPeriodEnd { get; set; }
		public int CurrentNewUsers { get; set; }
		public int PreviousNewUsers { get; set; }
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
