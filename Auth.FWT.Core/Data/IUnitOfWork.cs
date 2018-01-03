using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.FWT.Domain.Entities;
using Auth.FWT.Domain.Entities.API;
using Auth.FWT.Domain.Entities.Identity;

namespace Auth.FWT.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();

        int Commit();

        Task<int> CommitAsync();

        void Dispose(bool disposing);

        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey> where TKey : IComparable;

        void Rollback();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        IRepository<TelegramSession, int> TelegramSessionRepository { get; }

        IRepository<User, int> UserRepository { get; }

        IRepository<RefreshToken, string> RefreshTokenRepository { get; }

        IRepository<ClientAPI, string> ClientAPIRepository { get; }
    }
}