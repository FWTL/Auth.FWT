using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWT.Core.Services.KeyVault
{
    public interface IAzureKeyVault
    {
        Task<RsaSecurityKey> GetRsaKey(string keyId);

        Task<IDictionary<string, string>> ParseAsync()
    }
}
