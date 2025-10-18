using Microsoft.EntityFrameworkCore.Storage;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		/// <summary>
		/// Start a new database transaction.
		/// </summary>
		Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Commit the provided transaction.
		/// </summary>
		Task CommitAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

		/// <summary>
		/// Rollback the provided transaction.
		/// </summary>
		Task RollbackAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

		/// <summary>
		/// Save pending changes to the underlying DbContext.
		/// Returns true if save succeeded (no exception).
		/// </summary>
		Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}