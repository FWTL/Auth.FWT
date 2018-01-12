using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Domain.Entities.Identity;

namespace Auth.FWT.Core.Identity
{
    public interface IApplicationRoleManager : IDisposable
    {
        Task<IEnumerable<string>> CreateAsync(UserRole role);

        Task<IEnumerable<string>> DeleteAsync(byte roleId);

        Task<UserRole> FindByIdAsync(byte roleId);

        Task<UserRole> FindByNameAsync(string roleName);

        Task<IEnumerable<UserRole>> GetRolesAsync();

        Task<bool> RoleExistsAsync(string roleName);
    }
}
