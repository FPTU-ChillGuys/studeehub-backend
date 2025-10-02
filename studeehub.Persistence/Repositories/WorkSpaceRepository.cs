using Microsoft.EntityFrameworkCore;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class WorkSpaceRepository : GenericRepository<WorkSpace>, IWorkSpaceRepository
	{
		public WorkSpaceRepository(StudeeHubDBContext context) : base(context)
		{
		}

		public async Task<string> GenerateUniqueWorkspaceNameAsync(Guid userId)
		{
			const string baseName = "workspace";

			// Query DB for names starting with baseName for the given user,
			// extract numeric suffix, parse, and get max
			var workspaceNames = await _context.WorkSpaces
				.AsNoTracking()
				.Where(w => w.UserId == userId && EF.Functions.Like(w.Name, baseName + "%"))
				.Select(w => w.Name.Substring(baseName.Length))
				.ToListAsync();

			var maxNumber = workspaceNames
				.Where(suffix => !string.IsNullOrEmpty(suffix) && int.TryParse(suffix, out _))
				.Select(suffix => int.Parse(suffix))
				.DefaultIfEmpty(0)
				.Max();

			return $"{baseName}{maxNumber + 1}";
		}
	}
}
