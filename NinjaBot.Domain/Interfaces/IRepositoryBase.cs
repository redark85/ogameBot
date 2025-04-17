using System;

using System.Linq.Expressions;


namespace NinjaBot.Domain.Interfaces;

public interface IRepositoryBase<TEntity>
     where TEntity : class
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entity);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);

    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes);
    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entity);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    Task<bool> ExistsById(long id);

}
