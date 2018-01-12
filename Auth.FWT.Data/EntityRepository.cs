using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Domain.Entities;
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

        public void BatchDelete(Expression<Func<TEntity, bool>> predicate)
        {
            _dbEntitySet.Where(predicate).Delete();
        }

        public void Delete(TEntity entity, bool isHardDelete)
        {
            _context.SetAsDeleted<TEntity, TKey>(entity);
        }

        public Task Delete(TKey id, Expression<Func<TEntity, bool>> predicate, bool isHardDelete)
        {
            var query = _dbEntitySet.Where(predicate).Where(x => (object)x.Id == (object)id);
            query.Delete();

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

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate);
        }

        public IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities;
        }

        public TEntity GetSingle(TKey id)
        {
            return _dbEntitySet.Where(x => (object)x.Id == (object)id).FirstOrDefault();
        }

        public void IgnoreColumns(TEntity entity, params Expression<Action>[] @params)
        {
            _context.IgnoreUpdateEntityProperties<TEntity, TKey>(entity, @params);
        }

        public void Insert(TEntity entity)
        {
            _context.SetAsAdded<TEntity, TKey>(entity);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbEntitySet;
        }

        public void Update(TEntity entity)
        {
            _context.SetAsModified<TEntity, TKey>(entity);
        }

        public void UpdateColumns(TEntity entity, params Expression<Func<dynamic>>[] @params)
        {
            _context.UpdateEntityProperties<TEntity, TKey>(entity, @params);
        }

        private IQueryable<TEntity> IncludeProperties(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> entities = _dbEntitySet;
            foreach (var includeProperty in includeProperties)
            {
                entities = entities.Include(includeProperty);
            }

            return entities;
        }
    }
}