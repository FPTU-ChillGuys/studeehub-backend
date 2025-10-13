using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		public UserRepository(StudeeHubDBContext context) : base(context)
		{
		}

		public async Task<IList<string>> GetRolesAsync(Guid userId)
		{
			// Return normalized role names to match checks like "USER" / "ADMIN"
			var roleNames = await (from ur in _context.UserRoles
								   join r in _context.Roles on ur.RoleId equals r.Id
								   where ur.UserId == userId
								   select r.NormalizedName)
								   .ToListAsync();
			return roleNames;
		}

		public async Task<bool> IsAdminAsync(Guid userId)
		{
			// Compare against normalized name to be safe with casing in DB
			return await (from ur in _context.UserRoles
						  join r in _context.Roles on ur.RoleId equals r.Id
						  where ur.UserId == userId && r.NormalizedName == "ADMIN"
						  select ur)
						  .AnyAsync();
		}

		public async Task<bool> IsUserAsync(Guid userId)
		{
			// Compare against normalized name to be safe with casing in DB
			return await (from ur in _context.UserRoles
						  join r in _context.Roles on ur.RoleId equals r.Id
						  where ur.UserId == userId && r.NormalizedName == "USER"
						  select ur)
						  .AnyAsync();
		}

		public async Task<Dictionary<Guid, List<string>>> GetUserRolesAsync(IEnumerable<Guid> userIds)
		{
			if (userIds == null)
				return new Dictionary<Guid, List<string>>();

			var idsList = userIds as IList<Guid> ?? userIds.ToList();
			if (!idsList.Any())
				return new Dictionary<Guid, List<string>>();

			// Use a HashSet for faster lookup in the generated SQL/EF translation when possible
			var idsSet = idsList.ToHashSet();

			// Single server-side query that returns (UserId, NormalizedRoleName) pairs for the provided user ids
			var pairs = await (from ur in _context.UserRoles
							   join r in _context.Roles on ur.RoleId equals r.Id
							   where idsSet.Contains(ur.UserId)
							   select new { ur.UserId, RoleName = r.NormalizedName })
							  .ToListAsync();

			// Group into dictionary: userId -> list of normalized role names (distinct, non-empty, trimmed, case-insensitive)
			var dict = pairs
				.GroupBy(p => p.UserId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(x => x.RoleName)
						  .Where(n => !string.IsNullOrWhiteSpace(n))
						  .Select(n => n!.Trim())
						  .Distinct(StringComparer.OrdinalIgnoreCase)
						  .ToList()
				);

			// Ensure every requested id has an entry (empty list if no roles)
			foreach (var id in idsList)
			{
				if (!dict.ContainsKey(id))
					dict[id] = new List<string>();
			}

			return dict;
		}
	}
}
