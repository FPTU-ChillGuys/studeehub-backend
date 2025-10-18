using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
	{
		public ScheduleRepository(StudeeHubDBContext context) : base(context)
		{
		}

		/// <summary>
		/// Get upcoming schedules that should be reminded now.
		/// Caller should pass a consistent clock (preferably UTC).
		/// Note: per-row reminder time check is applied in-memory to avoid EF translation issues.
		/// </summary>
		public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesToRemindAsync(DateTime now)
		{
			// Determine server-side lookahead based on maximum ReminderMinutesBefore in DB.
			// Use a sensible fallback (60 minutes) if there are no schedules or the value is small.
			var maxReminder = await _context.Schedules
				.AsNoTracking()
				.Select(s => (int?)s.ReminderMinutesBefore)
				.MaxAsync() ?? 0;

			var lookaheadMinutes = Math.Max(maxReminder, 60); // at least 60 minutes lookahead

			// Server-side safe filter: schedules starting in the next lookahead and not yet reminded.
			var candidates = await _context.Schedules
				.AsNoTracking()
				.Include(s => s.User)
				.Where(s =>
					s.StartTime > now &&
					s.StartTime <= now.AddMinutes(lookaheadMinutes) && // starts within lookahead
					!s.IsReminded)
				.ToListAsync();

			// Apply per-row reminder window check in-memory (uses per-row ReminderMinutesBefore).
			return candidates.Where(s => now >= s.StartTime.AddMinutes(-s.ReminderMinutesBefore));
		}

		/// <summary>
		/// Get schedules that are currently ongoing and not checked-in yet.
		/// Caller should pass a consistent clock (preferably UTC).
		/// </summary>
		public async Task<IEnumerable<Schedule>> GetCheckinSchedulesToRemindAsync(DateTime now)
		{
			return await _context.Schedules
				.AsNoTracking()
				.Include(s => s.User)
				.Where(s =>
					s.StartTime <= now &&
					s.EndTime > now &&       // event still ongoing
					!s.IsCheckin)            // not yet checked in
				.ToListAsync();
		}
	}
}
