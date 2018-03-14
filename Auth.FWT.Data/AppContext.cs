using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Entities;
using Auth.FWT.Data.Base.Configuration;
using Auth.FWT.Data.Conventions;

namespace Auth.FWT.Data
{
    public class AppContext : DbContext, IEntitiesContext
    {
        private static readonly object Lock = new object();

        private ObjectContext _objectContext;

        private DbTransaction _transaction;

        public AppContext()
            : this("name=AppContext")
        {
        }

        public AppContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ////DbInterception.Add(new NLogCommandInterceptor());
            Database.Log = q => Debug.Write(q);

            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<TelegramCode> TelegramCodes { get; set; }

        public DbSet<TelegramSession> TelegramSessions { get; set; }

        public DbSet<TelegramJobData> TelegramJobData { get; set; }

        public DbSet<TelegramJob> TelegramJobs { get; set; }

        public void BeginTransaction()
        {
            _objectContext = ((IObjectContextAdapter)this).ObjectContext;
            if (_objectContext.Connection.State == ConnectionState.Open)
            {
                return;
            }

            _objectContext.Connection.Open();
            _transaction = _objectContext.Connection.BeginTransaction();
        }

        public void CollectionLoaded<TEntity, TKey, TElement>(TEntity entity, Expression<Func<TEntity, System.Collections.Generic.ICollection<TElement>>> navigationProperty)
            where TEntity : BaseEntity<TKey>
            where TElement : class
        {
            Entry(entity).Collection(navigationProperty).IsLoaded = true;
        }

        public int Commit()
        {
            var saveChanges = SaveChanges();
            _transaction.Commit();
            return saveChanges;
        }

        public Task<int> CommitAsync()
        {
            var saveChangesAsync = SaveChangesAsync();
            _transaction.Commit();
            return saveChangesAsync;
        }

        public void IgnoreUpdateEntityProperties<TEntity, TKey>(TEntity entity, params Expression<Action>[] @params) where TEntity : BaseEntity<TKey>
        {
            var dbEntityEntry = GetDbEntityEntrySafely<TEntity, TKey>(entity, EntityState.Modified);
            foreach (var param in @params)
            {
                MemberExpression expressionBody = (MemberExpression)param.Body;
                var name = expressionBody.Type.Name;

                dbEntityEntry.Property(name).IsModified = false;
            }
        }

        public bool IsCollectionLoaded<TEntity, TKey, TElement>(TEntity entity, Expression<Func<TEntity, System.Collections.Generic.ICollection<TElement>>> navigationProperty)
            where TEntity : BaseEntity<TKey>
            where TElement : class
        {
            var result = Entry(entity).Collection(navigationProperty).IsLoaded;
            return result;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public DbSet<TEntity> Set<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            return base.Set<TEntity>();
        }

        public void SetAsAdded<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>
        {
            UpdateEntityState<TEntity, TKey>(entity, EntityState.Added);
        }

        public void SetAsDeleted<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>
        {
            UpdateEntityState<TEntity, TKey>(entity, EntityState.Deleted);
        }

        public void SetAsDetached<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>
        {
            var local = this.Set<TEntity>().Local.FirstOrDefault(c => c.Id.Equals(entity.Id));
            if (local != null)
            {
                this.Entry(local).State = EntityState.Detached;
            }
        }

        public void SetAsModified<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>
        {
            UpdateEntityState<TEntity, TKey>(entity, EntityState.Modified);
        }

        public void UpdateEntityProperties<TEntity, TKey>(TEntity entity, params Expression<Func<dynamic>>[] @params) where TEntity : BaseEntity<TKey>
        {
            var dbEntityEntry = GetDbEntityEntrySafely<TEntity, TKey>(entity, EntityState.Modified);
            for (int i = 0; i < @params.Length; i++)
            {
                MemberExpression expressionBody = null;
                if (@params[i].Body is MemberExpression)
                {
                    expressionBody = (MemberExpression)@params[i].Body;
                }
                else
                {
                    var op = ((UnaryExpression)@params[i].Body).Operand;
                    expressionBody = ((MemberExpression)op);
                }

                var name = expressionBody.Member.Name;
                dbEntityEntry.Property(name).IsModified = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_objectContext != null && _objectContext.Connection.State == ConnectionState.Open)
                {
                    _objectContext.Connection.Close();
                }

                if (_objectContext != null)
                {
                    _objectContext.Dispose();
                    _objectContext = null;
                }

                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Add(new StringConventions());
            modelBuilder.Conventions.Add(new DecimalConventions());
            modelBuilder.Conventions.Add(new IndexesConventions());

            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserClaimConfiguration());
            modelBuilder.Configurations.Add(new UserLoginConfiguration());
            modelBuilder.Configurations.Add(new UserRoleConfiguration());

            modelBuilder.Configurations.Add(new ClientAPIConfiguration());
            modelBuilder.Configurations.Add(new RefreshTokenConfiguration());
        }

        private DbEntityEntry GetDbEntityEntrySafely<TEntity, TKey>(TEntity entity, EntityState state) where TEntity : BaseEntity<TKey>
        {
            var dbEntityEntry = Entry(entity);
            if (state == EntityState.Added)
            {
                return dbEntityEntry;
            }

            if (state == EntityState.Modified || state == EntityState.Deleted)
            {
                var local = this.Set<TEntity>().Local.FirstOrDefault(c => c.Id.Equals(entity.Id));
                if (local != null)
                {
                    this.Entry(local).State = EntityState.Detached;
                }

                Set<TEntity>().Attach(entity);
                return dbEntityEntry;
            }

            return dbEntityEntry;
        }

        private void UpdateEntityState<TEntity, TKey>(TEntity entity, EntityState entityState) where TEntity : BaseEntity<TKey>
        {
            var dbEntityEntry = GetDbEntityEntrySafely<TEntity, TKey>(entity, entityState);
            dbEntityEntry.State = entityState;
        }
    }
}