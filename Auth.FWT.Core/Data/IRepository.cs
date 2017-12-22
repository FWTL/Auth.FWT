using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels;

namespace Auth.FWT.Core.Data
{
    public interface IRepository<TEntity, TKey> : IDisposable
        where TEntity : BaseEntity<TKey>
        where TKey : IComparable
    {
        void BatchDelete(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false);

        void BatchDelete<TOrder>(Expression<Func<TEntity, bool>> predicate, OrderBy direction, Expression<Func<TEntity, TOrder>> orderBy = null, int take = 0) where TOrder : IComparable;

        void BatchUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> update, params Expression<Func<TEntity, object>>[] includeProperties);

        void BatchUpdate<TOrder>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateStatement, OrderBy direction, Expression<Func<TEntity, TOrder>> orderBy, int take) where TOrder : IComparable;

        void Delete(TEntity entity, bool isHardDelete = false);

        Task Delete(TKey id, Expression<Func<TEntity, bool>> predicate, bool isHardDelete = false);

        void Detach(ICollection<TEntity> collection);

        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(TKey id);

        void IgnoreColumns(TEntity entity, params Expression<Action>[] @params);

        void Insert(TEntity entity);

        IQueryable<TEntity> Query(bool includeDeleted = false);

        void Update(TEntity entity);

        void UpdateColumns(TEntity entity, params Expression<Func<dynamic>>[] @params);
    }
}
