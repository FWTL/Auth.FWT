using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWT.Core.Services.KeyVault
{
    public interface IAzureKeyVault
    {
        Task<RsaSecurityKey> GetRsaKeyAsync(string keyId);

        Task<IDictionary<string, string>> ParseAsync();
    }
}