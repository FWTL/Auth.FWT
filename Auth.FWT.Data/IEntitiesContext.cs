using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels;

namespace Auth.FWT.Data
{
    public interface IEntitiesContext : IDisposable
    {
        void BeginTransaction();

        void CollectionLoaded<TEntity, TKey, TElement>(TEntity entity, Expression<Func<TEntity, System.Collections.Generic.ICollection<TElement>>> navigationProperty)
            where TEntity : BaseEntity<TKey>
            where TElement : class;

        int Commit();

        Task<int> CommitAsync();

        void IgnoreUpdateEntityProperties<TEntity, TKey>(TEntity entity, params Expression<Action>[] @params) where TEntity : BaseEntity<TKey>;

        bool IsCollectionLoaded<TEntity, TKey, TElement>(TEntity entity, Expression<Func<TEntity, System.Collections.Generic.ICollection<TElement>>> navigationProperty)
            where TEntity : BaseEntity<TKey>
            where TElement : class;

        void Rollback();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbSet<TEntity> Set<TEntity, TKey>() where TEntity : BaseEntity<TKey>;

        void SetAsAdded<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>;

        void SetAsDeleted<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>;

        void SetAsDetached<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>;

        void SetAsModified<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>;

        void UpdateEntityProperties<TEntity, TKey>(TEntity entity, params Expression<Func<dynamic>>[] @params) where TEntity : BaseEntity<TKey>;
    }
}
