using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Domain.Entities.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationRoleStore : IRoleStore<UserRole, byte>, IQueryableRoleStore<UserRole, byte>
    {
        private readonly IEntitiesContext _dbContext;

        private bool _disposed;

        public ApplicationRoleStore(IEntitiesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserRole> Roles
        {
            get { return _dbContext.Set<UserRole, byte>().AsQueryable(); }
        }

        public virtual Task CreateAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsAdded<UserRole, byte>(role);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsDeleted<UserRole, byte>(role);
            return _dbContext.SaveChangesAsync();
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
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                }
            }

            _disposed = true;
        }

        public Task<UserRole> FindByIdAsync(byte roleId)
        {
            return _dbContext.Set<UserRole, byte>().FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public Task<UserRole> FindByNameAsync(string roleName)
        {
            return _dbContext.Set<UserRole, byte>().FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsModified<UserRole, byte>(role);
            return _dbContext.SaveChangesAsync();
        }
    }
}
