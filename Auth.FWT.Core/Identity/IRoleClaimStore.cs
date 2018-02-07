using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.Identity
{
    public interface IRoleClaimStore<TRole, in TKey> : IRoleStore<TRole, TKey>, IDisposable where TRole : class, IRole<TKey>
    {
        Task AddClaimAsync(TRole role, Claim claim);

        Task<IList<Claim>> GetClaimsAsync(TRole role);

        Task RemoveClaimAsync(TRole role, Claim claim);
    }
}
