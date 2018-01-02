using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Domain.Entities;
using Auth.FWT.Domain.Entities.Identity;

namespace Auth.FWT.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IEntitiesContext _context;

        private bool _disposed;

        private Hashtable _repositories;

        public UnitOfWork(IEntitiesContext context)
        {
            _context = context;
        }

        public IRepository<TelegramSession, int> TelegramSessionRepository
        {
            get
            {
                return Repository<TelegramSession, int>();
            }
        }

        public IRepository<User, int> UserRepository
        {
            get
            {
                return Repository<User, int>();
            }
        }

        public void BeginTransaction()
        {
            _context.BeginTransaction();
        }

        public int Commit()
        {
            return _context.Commit();
        }

        public Task<int> CommitAsync()
        {
            return _context.CommitAsync();
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
                if (_repositories != null)
                {
                    foreach (IDisposable repository in _repositories.Values)
                    {
                        repository.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        public IRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IComparable
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;
            if (_repositories.ContainsKey(type))
            {
                return (IRepository<TEntity, TKey>)_repositories[type];
            }

            var repositoryType = typeof(EntityRepository<,>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(new Type[] { typeof(TEntity), typeof(TKey) }), _context));
            return (IRepository<TEntity, TKey>)_repositories[type];
        }

        public void Rollback()
        {
            _context.Rollback();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}