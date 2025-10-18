using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IUserRepository : IGenericRepository<User>
	{
		/// <summary>
		/// Returns the role names assigned to the given user.
		/// </summary>
		Task<IList<string>> GetRolesAsync(Guid userId);

		/// <summary>
		/// Returns true if the user has the "admin" role.
		/// </summary>
		Task<bool> IsAdminAsync(Guid userId);

		/// <summary>
		/// Returns true if the user has the "user" role.
		/// </summary>
		Task<bool> IsUserAsync(Guid userId);

		/// <summary>
		/// Returns a mapping from userId -> role names for the provided user ids.
		/// Optimized to fetch roles for many users in a single query.
		/// </summary>
		Task<Dictionary<Guid, List<string>>> GetUserRolesAsync(IEnumerable<Guid> userIds);
	}
}
