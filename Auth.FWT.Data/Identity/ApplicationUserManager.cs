using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Identity;
using Auth.FWT.Domain.Entities.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationUserManager : IApplicationUserManager<int>
    {
        private readonly IAuthenticationManager _authenticationManager;

        private readonly UserManager<User, int> _userManager;

        private bool _disposed;

        public ApplicationUserManager(UserManager<User, int> userManager, IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }

        public string ApplicationCookie
        {
            get
            {
                return DefaultAuthenticationTypes.ApplicationCookie;
            }
        }

        public string ExternalCookie
        {
            get
            {
                return DefaultAuthenticationTypes.ExternalCookie;
            }
        }

        public Task<IdentityResult> AddLoginAsync(int userId, UserLoginInfo login)
        {
            return _userManager.AddLoginAsync(userId, login);
        }

        public Task<IdentityResult> AddToRoleAsync(int userId, Auth.FWT.Core.Enums.Enum.UserRole role)
        {
            return _userManager.AddToRoleAsync(userId, role.ToString());
        }

        public async Task<SignInStatus> CanSignInPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user.IsNull())
            {
                return SignInStatus.Failure;
            }

            if (await CheckPasswordAsync(user, password).ConfigureAwait(false))
            {
                return SignInStatus.Success;
            }

            return SignInStatus.Failure;
        }

        public void Challenge(string redirectUri, string xsrfKey, int? userId, params string[] authenticationTypes)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            if (userId != null)
            {
                properties.Dictionary[xsrfKey] = userId.ToString();
            }

            _authenticationManager.Challenge(properties, authenticationTypes);
        }

        public Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(userId, currentPassword, newPassword);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user.IsNull())
            {
                throw new ArgumentNullException("user is null");
            }

            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<IdentityResult> CreateAsync(User user)
        {
            return _userManager.CreateAsync(user);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            return _userManager.CreateIdentityAsync(user, authenticationType);
        }

        public ClaimsIdentity CreateTwoFactorRememberBrowserIdentity(int userId)
        {
            return _authenticationManager.CreateTwoFactorRememberBrowserIdentity(userId.ToString());
        }

        public async Task<IdentityResult> DeleteAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            return await _userManager.DeleteAsync(user);
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
                if (_userManager != null)
                {
                    _userManager.Dispose();
                }
            }

            _disposed = true;
        }

        public async Task<SignInStatus> ExternalSignIn(ApplicationExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await FindAsync(loginInfo.Login).ConfigureAwait(false);
            if (user.IsNull())
            {
                return SignInStatus.Failure;
            }

            await SignInAsync(user, isPersistent, rememberBrowser: false);
            return SignInStatus.Success;
        }

        public virtual async Task<User> FindAsync(UserLoginInfo login)
        {
            return await _userManager.FindAsync(login);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<User> FindByIdAsync(int userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(int userId)
        {
            return _userManager.GenerateEmailConfirmationTokenAsync(userId);
        }

        public Task<string> GenerateResetPasswordCodeAsync(int userId)
        {
            return _userManager.GeneratePasswordResetTokenAsync(userId);
        }

        public async Task<ApplicationExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            var result = await _authenticationManager.GetExternalLoginInfoAsync();
            if (result != null)
            {
                return new ApplicationExternalLoginInfo()
                {
                    DefaultUserName = result.DefaultUserName,
                    Email = result.Email,
                    ExternalIdentity = result.ExternalIdentity,
                    Login = result.Login
                };
            }

            return null;
        }

        public Task<IList<string>> GetRolesAsync(int userId)
        {
            return _userManager.GetRolesAsync(userId);
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _userManager.Users.ToListAsync();
        }

        public Task<bool> IsInRoleAsync(int userId, string role)
        {
            return _userManager.IsInRoleAsync(userId, role);
        }

        public Task<IdentityResult> RemoveFromRoleAsync(int userId, string role)
        {
            return _userManager.RemoveFromRoleAsync(userId, role);
        }

        public Task<IdentityResult> ResetPasswordAsync(int userId, string code, string password)
        {
            return _userManager.ResetPasswordAsync(userId, code, password);
        }

        public async Task SignInAsync(User user, bool isPersistent, bool rememberBrowser)
        {
            if (user.IsNull())
            {
                throw new ArgumentNullException("user is null");
            }

            SignOut(this.ExternalCookie, this.ApplicationCookie);
            var userIdentity = await CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie).ConfigureAwait(false);
            ////userIdentity.AddClaim(new Claim(ClaimTypes.UserData, user.AccountId.ToString()));

            if (rememberBrowser)
            {
                var rememberBrowserIdentity = CreateTwoFactorRememberBrowserIdentity(user.Id);
                _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent, IssuedUtc = DateTime.UtcNow, ExpiresUtc = DateTime.UtcNow.AddDays(14) }, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
            }
        }

        public async Task<SignInStatus> SignInPasswordAsync(string email, string password, bool isPersistent, bool rememberBrowser)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user.IsNull())
            {
                return SignInStatus.Failure;
            }

            if (await CheckPasswordAsync(user, password).ConfigureAwait(false))
            {
                await SignInAsync(user, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }

            return SignInStatus.Failure;
        }

        public void SignOut(params string[] authenticationTypes)
        {
            _authenticationManager.SignOut(authenticationTypes);
        }

        public Task<IdentityResult> UpdateAsync(User user)
        {
            return _userManager.UpdateAsync(user);
        }
    }
}
