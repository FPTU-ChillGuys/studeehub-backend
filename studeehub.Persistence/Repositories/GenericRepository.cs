using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Persistence.Context;
using System.Linq.Expressions;

namespace studeehub.Persistence.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly StudeeHubDBContext _context;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(StudeeHubDBContext context) : base()
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public async Task<IEnumerable<T>> GetAllAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			bool asNoTracking = false)
		{
			IQueryable<T> query = _dbSet;

			if (filter is not null)
				query = query.Where(filter);

			if (include is not null)
				query = include(query);

			if (orderBy is not null)
				query = orderBy(query);

			if (asNoTracking)
				query = query.AsNoTracking();

			return await query.ToListAsync();
		}

		public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			int pageNumber = 1,
			int pageSize = 10,
			bool asNoTracking = false)
		{
			IQueryable<T> query = _dbSet;

			if (filter is not null)
				query = query.Where(filter);

			if (include is not null)
				query = include(query);

			if (asNoTracking)
				query = query.AsNoTracking();

			if (orderBy is not null)
				query = orderBy(query); // apply sorting

			int totalCount = await query.CountAsync();

			var items = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<T?> GetByConditionAsync(
			Expression<Func<T, bool>> predicate,
			Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
			bool asNoTracking = false)
		{
			IQueryable<T> query = _dbSet;

			if (include is not null)
				query = include(query);

			if (asNoTracking)
				query = query.AsNoTracking();

			return await query.FirstOrDefaultAsync(predicate);
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public async Task AddRangeAsync(IEnumerable<T> entities)
		{
			await _dbSet.AddRangeAsync(entities);
		}

		public void Update(T entity)
		{
			_dbSet.Update(entity);
		}

		public void Remove(T entity)
		{
			_dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			_dbSet.RemoveRange(entities);
		}

		public async Task<bool> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		//public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
		//{
		//    IQueryable<T> query = _dbSet;

		//    if (filter is not null)
		//        query = query.Where(filter);

		//    return await query.CountAsync();
		//}

		public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.AnyAsync(predicate);
		}
	}
}
