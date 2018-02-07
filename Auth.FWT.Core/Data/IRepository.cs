using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Entities;

namespace Auth.FWT.Core.Data
{
    public interface IRepository<TEntity, TKey> : IDisposable
        where TEntity : BaseEntity<TKey>
        where TKey : IComparable
    {
        void BatchDelete(Expression<Func<TEntity, bool>> predicate);

        void Delete(TEntity entity);

        Task Delete(TKey id, Expression<Func<TEntity, bool>> predicate);

        void Detach(ICollection<TEntity> collection);

        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(TKey id);

        Task<TEntity> GetSingleAsync(TKey id);

        void IgnoreColumns(TEntity entity, params Expression<Action>[] @params);

        void Insert(TEntity entity);

        IQueryable<TEntity> Query();

        void Update(TEntity entity);

        void UpdateColumns(TEntity entity, params Expression<Func<dynamic>>[] @params);
    }
}
