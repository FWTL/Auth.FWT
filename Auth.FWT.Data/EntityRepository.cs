using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.DomainModels;
using Z.EntityFramework.Plus;

namespace Auth.FWT.Data
{
    public class EntityRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>, new()
        where TKey : IComparable
    {
        private readonly IEntitiesContext _context;

        private readonly DbSet<TEntity> _dbEntitySet;

        private bool _disposed;

        public EntityRepository(IEntitiesContext context)
        {
            _context = context;
            _dbEntitySet = _context.Set<TEntity, TKey>();
        }

        public void BatchDelete(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false)
        {
            if (hardDelete)
            {
                _dbEntitySet.Where(predicate).Where(x => x.IsDeleted == false).Update(x => new TEntity { IsDeleted = true, DeleteDateUTC = DateTime.UtcNow });
            }
            else
            {
                _dbEntitySet.Where(predicate).Where(x => x.IsDeleted == false).Where(predicate).Delete();
            }
        }

        public void BatchDelete<TOrder>(Expression<Func<TEntity, bool>> predicate, OrderBy direction, Expression<Func<TEntity, TOrder>> orderBy, int take) where TOrder : IComparable
        {
            BulkOperation<TOrder>(predicate, x => new TEntity { IsDeleted = true, DeleteDateUTC = DateTime.UtcNow }, direction, orderBy, take);
        }

        public void BatchUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> update, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = IncludeProperties(includeProperties);
            query.Where(x => x.IsDeleted == false).Update(update);
        }

        public void BatchUpdate<TOrder>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateStatement, OrderBy direction, Expression<Func<TEntity, TOrder>> orderBy, int take) where TOrder : IComparable
        {
            BulkOperation(predicate, updateStatement, direction, orderBy, take);
        }

        public void Delete(TEntity entity, bool isHardDelete)
        {
            if (!isHardDelete)
            {
                entity.IsDeleted = true;
                entity.DeleteDateUTC = DateTime.UtcNow;
                UpdateColumns(entity, () => entity.IsDeleted, () => entity.DeleteDateUTC);
            }
            else
            {
                _context.SetAsDeleted<TEntity, TKey>(entity);
            }
        }

        public Task Delete(TKey id, Expression<Func<TEntity, bool>> predicate, bool isHardDelete)
        {
            var query = _dbEntitySet.Where(predicate).Where(x => x.IsDeleted == false && (object)x.Id == (object)id);
            if (isHardDelete)
            {
                query.Delete();
            }
            else
            {
                query.Update(x => new TEntity { IsDeleted = true, DeleteDateUTC = DateTime.UtcNow });
            }

            return Task.FromResult(0);
        }

        public void Detach(ICollection<TEntity> collection)
        {
            foreach (var entity in collection)
            {
                _context.SetAsDetached<TEntity, TKey>(entity);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }

        public IEnumerable<TEntity> FindBy(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.AsNoTracking().Where(predicate).Where(x => x.IsDeleted == false);
        }

        public IEnumerable<TEntity> GetAllIncluding(params System.Linq.Expressions.Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities;
        }

        public TEntity GetSingle(TKey id)
        {
            return _dbEntitySet.AsNoTracking().Where(x => x.IsDeleted == false && (object)x.Id == (object)id).FirstOrDefault();
        }

        public void IgnoreColumns(TEntity entity, params Expression<Action>[] @params)
        {
            _context.IgnoreUpdateEntityProperties<TEntity, TKey>(entity, @params);
        }

        public void Insert(TEntity entity)
        {
            _context.SetAsAdded<TEntity, TKey>(entity);
        }

        public IQueryable<TEntity> Query(bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return _dbEntitySet;
            }

            return _dbEntitySet.Where(x => x.IsDeleted == false);
        }

        public void Update(TEntity entity)
        {
            _context.SetAsModified<TEntity, TKey>(entity);
        }

        public void UpdateColumns(TEntity entity, params Expression<Func<dynamic>>[] @params)
        {
            _context.UpdateEntityProperties<TEntity, TKey>(entity, @params);
        }

        private void BulkOperation<TOrder>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateStatement, OrderBy direction, Expression<Func<TEntity, TOrder>> orderBy, int take) where TOrder : IComparable
        {
            var query = _dbEntitySet.Where(predicate).Where(x => x.IsDeleted == false);
            if (orderBy != null)
            {
                if (direction == OrderBy.Ascending)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            query.Update(updateStatement);
        }

        private IQueryable<TEntity> FilterQuery(Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            entities = (predicate != null) ? entities.Where(predicate) : entities;
            entities = (orderBy == OrderBy.Ascending)
                ? entities.OrderBy(keySelector)
                : entities.OrderByDescending(keySelector);
            return entities.Where(x => x.IsDeleted == false);
        }

        private IQueryable<TEntity> IncludeProperties(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> entities = _dbEntitySet;
            foreach (var includeProperty in includeProperties)
            {
                entities = entities.Include(includeProperty).Where(x => x.IsDeleted == false);
            }

            return entities;
        }
    }
}
