using Microsoft.EntityFrameworkCore;
using NinjaBot.Domain.Interfaces;
using NinjaBot.Domain.Utils;
using System.Linq.Expressions;

namespace NinjaBot.Infrastructure.Persistence
{
    public abstract class RepositoryBase<TEntity, TDbContext> : IRepositoryBase<TEntity>
    where TEntity : class
    where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(TDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null) => predicate != null ? await _dbSet.CountAsync(predicate) : await _dbSet.CountAsync();

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) => predicate != null ? await _dbSet.AnyAsync(predicate) : await _dbSet.AnyAsync();

        public virtual void Remove(TEntity entity) => _dbSet.Remove(entity);

        public virtual void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public virtual void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return filter != null ? await query.AsNoTracking().FirstOrDefaultAsync(filter) : await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes.Length == 0)
                query = includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual Task<bool> ExistsById(long id)
        {
            Check.NotEmpty(id, nameof(id));

            Type entityType = typeof(TEntity);

            return ((IQueryable<IBaseEntity>)_dbSet).AnyAsync(m => m.Id == id);
        }
    }
}
