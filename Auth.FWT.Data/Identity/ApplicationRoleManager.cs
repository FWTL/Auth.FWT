using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.Identity;
using Auth.FWT.Domain.Entities.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationRoleManager : IApplicationRoleManager
    {
        private readonly RoleManager<UserRole, int> _roleManager;

        private bool _disposed;

        public ApplicationRoleManager(RoleManager<UserRole, int> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> CreateAsync(UserRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("Role is null");
            }

            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                return new List<string>() { "Role already exists" };
            }

            var result = await _roleManager.CreateAsync(role);
            return result.Errors;
        }

        public async Task<IEnumerable<string>> DeleteAsync(byte roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return new List<string>() { "Role not exists" };
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                return result.Errors;
            }
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
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                }
            }

            _disposed = true;
        }

        public async Task<UserRole> FindByIdAsync(byte roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<UserRole> FindByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<IEnumerable<UserRole>> GetRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.ToList());
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}