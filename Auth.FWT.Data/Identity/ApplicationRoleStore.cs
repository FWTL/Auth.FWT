using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.FWT.Core.Identity;
using Auth.FWT.Domain.Entities.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationRoleStore : IRoleStore<UserRole, int>, IQueryableRoleStore<UserRole, int>, IRoleClaimStore<UserRole, int>
    {
        private readonly IEntitiesContext _dbContext;

        private bool _disposed;

        public ApplicationRoleStore(IEntitiesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserRole> Roles
        {
            get { return _dbContext.Set<UserRole, int>().AsQueryable(); }
        }

        public virtual Task AddClaimAsync(UserRole role, Claim claim)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("roleClaim");
            }

            _dbContext.Set<RoleClaim, int>().Add(new RoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        public virtual async Task RemoveClaimAsync(UserRole role, Claim claim)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            IEnumerable<RoleClaim> claims;
            var claimValue = claim.Value;
            var claimType = claim.Type;
            if (AreClaimsLoaded(role))
            {
                claims = role.Claims.Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType).ToList();
            }
            else
            {
                var roleId = role.Id;
                claims = await _dbContext.Set<RoleClaim, int>().Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType && uc.RoleId.Equals(role)).ToListAsync();
            }

            foreach (var c in claims)
            {
                _dbContext.Set<RoleClaim, int>().Remove(c);
            }
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await EnsureClaimsLoaded(role);
            return role.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        private bool AreClaimsLoaded(UserRole role)
        {
            return _dbContext.IsCollectionLoaded<UserRole, int, RoleClaim>(role, x => x.Claims);
        }

        private async Task EnsureClaimsLoaded(UserRole role)
        {
            if (!AreClaimsLoaded(role))
            {
                var roleId = role.Id;
                await _dbContext.Set<RoleClaim, int>().Where(uc => uc.RoleId.Equals(roleId)).LoadAsync();
                _dbContext.CollectionLoaded<UserRole, int, RoleClaim>(role, x => x.Claims);
            }
        }

        public virtual Task CreateAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsAdded<UserRole, int>(role);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsDeleted<UserRole, int>(role);
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

        public Task<UserRole> FindByIdAsync(int roleId)
        {
            return _dbContext.Set<UserRole, int>().FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public Task<UserRole> FindByNameAsync(string roleName)
        {
            return _dbContext.Set<UserRole, int>().FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _dbContext.SetAsModified<UserRole, int>(role);
            return _dbContext.SaveChangesAsync();
        }
    }
}