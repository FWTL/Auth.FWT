using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.Identity
{
    public interface IApplicationUserManager<T> : IDisposable where T : struct
    {
        string ApplicationCookie { get; }

        string ExternalCookie { get; }

        Task<IdentityResult> AddLoginAsync(T userId, UserLoginInfo login);

        Task<IdentityResult> AddToRoleAsync(T userId, Enums.Enum.UserRole role);

        Task<SignInStatus> CanSignInPasswordAsync(string email, string password);

        void Challenge(string redirectUri, string xsrfKey, T? userId, params string[] authenticationTypes);

        Task<IdentityResult> ChangePasswordAsync(T userId, string currentPassword, string newPassword);

        Task<IdentityResult> CreateAsync(User user);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> DeleteAsync(T userId);

        Task<SignInStatus> ExternalSignIn(ApplicationExternalLoginInfo loginInfo, bool isPersistent);

        Task<User> FindByIdAsync(T userId);

        Task<string> GenerateEmailConfirmationTokenAsync(int userId);

        Task<string> GenerateResetPasswordCodeAsync(int userId);

        Task<ApplicationExternalLoginInfo> GetExternalLoginInfoAsync();

        Task<IList<string>> GetRolesAsync(T userId);

        Task<List<User>> GetUsersAsync();

        Task<bool> IsInRoleAsync(T userId, string role);

        Task<IdentityResult> RemoveFromRoleAsync(T userId, string role);

        Task<IdentityResult> ResetPasswordAsync(int userId, string code, string password);

        Task SignInAsync(User user, bool isPersistent, bool rememberBrowser);

        Task<SignInStatus> SignInPasswordAsync(string email, string password, bool isPersistent, bool rememberBrowser);

        void SignOut(params string[] authenticationTypes);

        Task<IdentityResult> UpdateAsync(User userId);
    }
}
