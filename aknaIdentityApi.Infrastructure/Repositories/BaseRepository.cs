
using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Interfaces.Repositories;
using aknaIdentityApi.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace aknaIdentityApi.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        protected readonly AknaIdentityDbContext context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(AknaIdentityDbContext context)
        {
            this.context = context;
            _dbSet = context.Set<TEntity>();
        }

        // Create Operations
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            await _dbSet.AddRangeAsync(entities);
        }

        // Read Operations
        public virtual async Task<TEntity?> GetByIdAsync(long id, bool trackChanges = true)
        {
            if (!trackChanges)
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            }
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity?> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool trackChanges = true,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            // Apply includes
            query = includes.Aggregate(query,
                (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool trackChanges = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            return predicate != null
                ? query.Where(predicate)
                : query;
        }

        public virtual async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool trackChanges = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            return predicate != null
                ? await query.Where(predicate).ToListAsync()
                : await query.ToListAsync();
        }

        // Count and Existence
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate != null
                ? await _dbSet.CountAsync(predicate)
                : await _dbSet.CountAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // Update Operations
        public virtual void Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            _dbSet.UpdateRange(entities);
        }

        // Delete Operations
        public virtual void Delete(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> SoftDeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.UtcNow;
            Update(entity);
            return true;
        }

        // Complex Query Methods
        public virtual IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<List<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int pageNumber = 1,
            int pageSize = 10,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            // Apply predicate
            if (predicate != null)
                query = query.Where(predicate);

            // Apply includes
            query = includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Apply ordering
            if (orderBy != null)
                query = orderBy(query);

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
