using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Responses.User;
using studeehub.Domain.Entities;
using System.Globalization;

namespace studeehub.Application.Extensions
{
    public static class UserMetricsHelper
    {
        public static (DateTime Start, DateTime End) ResolveDateRange(GetUserMetricsRequest request, DateTime now)
        {
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value.Date;
                var end = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                return (start, end);
            }

            return ResolveDateRange(request.Period, now);
        }

        public static (DateTime Start, DateTime End) ResolveDateRange(string? period, DateTime now)
        {
            return (period ?? "month").ToLower() switch
            {
                "week" => GetWeekRange(now),
                "year" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
                _ => (new DateTime(now.Year, now.Month, 1),
                      new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1))
            };
        }

        public static (DateTime Start, DateTime End) GetWeekRange(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            var start = date.AddDays(-diff).Date;
            return (start, start.AddDays(7).AddTicks(-1));
        }

        public static List<MonthlyUserCount> GroupUserGrowth(IEnumerable<User> users, string groupBy)
        {
            return groupBy switch
            {
                "day" => users
                    .GroupBy(u => u.CreatedAt.Date)
                    .Select(g => new MonthlyUserCount
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToList(),

                "week" => users
                    .GroupBy(u => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        u.CreatedAt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(g => new MonthlyUserCount
                    {
                        Year = g.Min(u => u.CreatedAt.Year),
                        Week = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Week)
                    .ToList(),

                _ => users
                    .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                    .Select(g => new MonthlyUserCount
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToList(),
            };
        }
    }
}
