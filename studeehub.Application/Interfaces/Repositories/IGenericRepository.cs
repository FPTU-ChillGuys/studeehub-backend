using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			bool asNoTracking = false
		);

		Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			int pageNumber = 1,
			int pageSize = 10,
			bool asNoTracking = false
		);

		Task<T?> GetByConditionAsync(
			Expression<Func<T, bool>> predicate,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			bool asNoTracking = false
		);


		Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
		Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
		Task AddAsync(T entity);
		Task AddRangeAsync(IEnumerable<T> entities);
		void Update(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
		Task<bool> SaveChangesAsync();
	}
}
