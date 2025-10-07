using Microsoft.EntityFrameworkCore.Storage;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Persistence.Context;

namespace studeehub.Persistence.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StudeeHubDBContext _context;

		public UnitOfWork(StudeeHubDBContext context)
		{
			_context = context;
		}

		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Database.BeginTransactionAsync(cancellationToken);
		}

		public async Task CommitAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
		{
			if (transaction == null) return;
			await transaction.CommitAsync(cancellationToken);
#if NET6_0_OR_GREATER
			await transaction.DisposeAsync();
#else
			transaction.Dispose();
#endif
		}

		public async Task RollbackAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
		{
			if (transaction == null) return;
			await transaction.RollbackAsync(cancellationToken);
#if NET6_0_OR_GREATER
			await transaction.DisposeAsync();
#else
			transaction.Dispose();
#endif
		}

		public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var result = await _context.SaveChangesAsync(cancellationToken);
			// return true when SaveChanges completed (result >= 0). Adjust if you expect >0 only.
			return result >= 0;
		}

		public void Dispose()
		{
			_context?.Dispose();
		}
	}
}