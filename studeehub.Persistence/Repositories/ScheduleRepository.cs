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
			// Server-side safe filter: schedules starting in the next hour and not yet reminded.
			var candidates = await _context.Schedules
				.AsNoTracking()
				.Include(s => s.User)
				.Where(s =>
					s.StartTime > now &&
					s.StartTime <= now.AddMinutes(60) && // starts within an hour
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
