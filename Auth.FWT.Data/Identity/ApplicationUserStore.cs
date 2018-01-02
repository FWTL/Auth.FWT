using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationUserStore
        : IUserStore<User, int>,
        IUserRoleStore<User, int>,
        IQueryableUserStore<User, int>,
        IUserSecurityStampStore<User, int>,
        IUserClaimStore<User, int>
    {
        private readonly IEntitiesContext _dbContext;

        private bool _disposed;

        public ApplicationUserStore(IEntitiesContext dbContext)
        {
            _dbContext = dbContext;
            AutoSaveChanges = true;
        }

        public bool AutoSaveChanges { get; set; }

        public bool DisposeContext { get; set; }

        public IQueryable<User> Users
        {
            get { return _dbContext.Set<User, int>(); }
        }

        public virtual Task AddClaimAsync(User user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            _dbContext.Set<UserClaim, int>().Add(new UserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        public virtual Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            _dbContext.Set<UserLogin, int>().Add(new UserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });
            return Task.FromResult(0);
        }

        public virtual async Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Can't be empty", "roleName");
            }

            var roleEntity = await _dbContext.Set<UserRole, byte>().SingleOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper());
            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role not found", roleName));
            }

            user.Roles.Add(roleEntity);
        }

        public virtual async Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _dbContext.SetAsAdded<User, int>(user);
            await SaveChanges();
        }

        public virtual async Task DeleteAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _dbContext.SetAsDeleted<User, int>(user);
            await SaveChanges();
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

        //// IUserLogin implementation
        public virtual async Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var userLogin =
                await _dbContext.Set<UserLogin, int>().FirstOrDefaultAsync(l => l.LoginProvider == provider && l.ProviderKey == key);
            if (userLogin != null)
            {
                var userId = userLogin.UserId;
                return await GeUserAggregateAsync(u => u.Id.Equals(userId));
            }

            return null;
        }

        public virtual Task<User> FindByIdAsync(int userId)
        {
            return GeUserAggregateAsync(u => u.Id.Equals(userId));
        }

        public virtual Task<User> FindByNameAsync(string userName)
        {
            return GeUserAggregateAsync(u => u.UserName.ToUpper() == userName.ToUpper());
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await EnsureClaimsLoaded(user);
            return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public virtual Task<bool> GetLockoutEnabledAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public virtual async Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userId = user.Id;
            return await _dbContext.Set<User, int>().Where(x => x.Id == user.Id).Include(x => x.Roles).SelectMany(x => x.Roles).Select(x => x.Name).ToListAsync();
        }

        public virtual Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("No empty role name", "roleName");
            }

            return await _dbContext.Set<User, int>().Where(x => x.Id == user.Id).Include(x => x.Roles).SelectMany(x => x.Roles).Where(x => x.Name == roleName).AnyAsync();
        }

        public virtual async Task RemoveClaimAsync(User user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            IEnumerable<UserClaim> claims;
            var claimValue = claim.Value;
            var claimType = claim.Type;
            if (AreClaimsLoaded(user))
            {
                claims = user.Claims.Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType).ToList();
            }
            else
            {
                var userId = user.Id;
                claims = await _dbContext.Set<UserClaim, int>().Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType && uc.UserId.Equals(userId)).ToListAsync();
            }

            foreach (var c in claims)
            {
                _dbContext.Set<UserClaim, int>().Remove(c);
            }
        }

        public virtual async Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Can't be empty", "roleName");
            }

            var roleEntity = await _dbContext.Set<UserRole, byte>().SingleOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper());
            if (roleEntity != null)
            {
                var roleId = roleEntity.Id;
                var userId = user.Id;
                var userRole = await _dbContext.Set<User, int>().Where(x => x.Id == user.Id).Include(x => x.Roles).SelectMany(x => x.Roles).Where(x => x.Name == roleName).FirstOrDefaultAsync();
                if (userRole != null)
                {
                    _dbContext.Set<UserRole, byte>().Remove(userRole);
                }
            }
        }

        public virtual Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public virtual Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public virtual async Task UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _dbContext.SetAsModified<User, int>(user);
            await SaveChanges();
        }

        protected virtual async Task<User> GeUserAggregateAsync(Expression<Func<User, bool>> filter)
        {
            int id;
            User user;
            if (FindByIdFilterParser.TryMatchAndGetId(filter, out id))
            {
                user = await Users.FirstOrDefaultAsync(filter);
            }
            else
            {
                user = await Users.FirstOrDefaultAsync(filter);
            }

            if (user != null)
            {
                await EnsureClaimsLoaded(user);
                await EnsureRolesLoaded(user);
            }

            return user;
        }

        private bool AreClaimsLoaded(User user)
        {
            return _dbContext.IsCollectionLoaded<User, int, UserClaim>(user, x => x.Claims);
        }

        private async Task EnsureClaimsLoaded(User user)
        {
            if (!AreClaimsLoaded(user))
            {
                var userId = user.Id;
                await _dbContext.Set<UserClaim, int>().Where(uc => uc.UserId.Equals(userId)).LoadAsync();
                _dbContext.CollectionLoaded<User, int, UserRole>(user, x => x.Roles);
            }
        }

        private async Task EnsureRolesLoaded(User user)
        {
            if (!_dbContext.IsCollectionLoaded<User, int, UserRole>(user, x => x.Roles))
            {
                var userId = user.Id;
                await _dbContext.Set<UserClaim, int>().Where(uc => uc.UserId.Equals(userId)).LoadAsync();
                _dbContext.CollectionLoaded<User, int, UserRole>(user, x => x.Roles);
            }
        }

        //////// Only call save changes if AutoSaveChanges is true
        private async Task SaveChanges()
        {
            if (AutoSaveChanges)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        //////// We want to use FindAsync() when looking for an User.Id instead of LINQ to avoid extra
        //////// database roundtrips. This class cracks open the filter expression passed by
        //////// UserStore.FindByIdAsync() to obtain the value of the id we are looking for
        private static class FindByIdFilterParser
        {
            //////// method we need to match: Object.Equals()
            private static readonly MethodInfo EqualsMethodInfo = ((MethodCallExpression)Predicate.Body).Method;

            //////// expression pattern we need to match
            private static readonly Expression<Func<User, bool>> Predicate = u => u.Id.Equals(default(int));

            //////// property access we need to match: User.Id
            private static readonly MemberInfo UserIdMemberInfo = ((MemberExpression)((MethodCallExpression)Predicate.Body).Object).Member;

            internal static bool TryMatchAndGetId(Expression<Func<User, bool>> filter, out int id)
            {
                //////// default value in case we can’t obtain it
                id = default(int);

                //// lambda body should be a call
                if (filter.Body.NodeType != ExpressionType.Call)
                {
                    return false;
                }

                //// actually a call to object.Equals(object)
                var callExpression = (MethodCallExpression)filter.Body;
                if (callExpression.Method != EqualsMethodInfo)
                {
                    return false;
                }
                //// left side of Equals() should be an access to User.Id
                if (callExpression.Object == null
                    || callExpression.Object.NodeType != ExpressionType.MemberAccess
                    || ((MemberExpression)callExpression.Object).Member != UserIdMemberInfo)
                {
                    return false;
                }

                //// There should be only one argument for Equals()
                if (callExpression.Arguments.Count != 1)
                {
                    return false;
                }

                MemberExpression fieldAccess;
                if (callExpression.Arguments[0].NodeType == ExpressionType.Convert)
                {
                    //// convert node should have an member access access node
                    //// This is for cases when primary key is a value type
                    var convert = (UnaryExpression)callExpression.Arguments[0];
                    if (convert.Operand.NodeType != ExpressionType.MemberAccess)
                    {
                        return false;
                    }

                    fieldAccess = (MemberExpression)convert.Operand;
                }
                else if (callExpression.Arguments[0].NodeType == ExpressionType.MemberAccess)
                {
                    //// Get field member for when key is reference type
                    fieldAccess = (MemberExpression)callExpression.Arguments[0];
                }
                else
                {
                    return false;
                }

                //// and member access should be a field access to a variable captured in a closure
                if (fieldAccess.Member.MemberType != MemberTypes.Field
                    || fieldAccess.Expression.NodeType != ExpressionType.Constant)
                {
                    return false;
                }

                //// expression tree matched so we can now just get the value of the id
                var fieldInfo = (FieldInfo)fieldAccess.Member;
                var closure = ((ConstantExpression)fieldAccess.Expression).Value;

                id = (int)fieldInfo.GetValue(closure);
                return true;
            }
        }
    }
}
