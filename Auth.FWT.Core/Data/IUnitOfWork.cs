using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Entities.API;
using Auth.FWT.Core.Entities.Identity;

namespace Auth.FWT.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ClientAPI, string> ClientAPIRepository { get; }

        IRepository<RefreshToken, string> RefreshTokenRepository { get; }

        IRepository<RoleClaim, int> RoleClaimRepository { get; }

        IRepository<UserRole, int> RoleRepository { get; }

        IRepository<TelegramCode, string> TelegramCodeRepository { get; }

        IRepository<TelegramSession, int> TelegramSessionRepository { get; }

        IRepository<User, int> UserRepository { get; }

        IRepository<TelegramJob, long> TelegramJobRepository { get; }

        void BeginTransaction();

        int Commit();

        Task<int> CommitAsync();

        void Dispose(bool disposing);

        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey> where TKey : IComparable;

        void Rollback();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
