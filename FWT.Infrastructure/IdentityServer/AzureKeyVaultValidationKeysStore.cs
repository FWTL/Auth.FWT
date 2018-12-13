using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FWT.Infrastructure.IdentityServer
{
    public class AzureKeyVaultValidationKeysStore : IValidationKeysStore
    {
        private readonly IMemoryCache _cache;
        private readonly KeyVaultClient _keyVaultClient;
        private readonly string _vault;
        private readonly string _certificateName;

        public AzureKeyVaultValidationKeysStore(IMemoryCache memoryCache, KeyVaultClient keyVaultClient, string vault, string certificateName)
        {
            _cache = memoryCache;
            _keyVaultClient = keyVaultClient;
            _vault = vault;
            _certificateName = certificateName;
        }

        public async Task<IEnumerable<SecurityKey>> GetValidationKeysAsync()
        {
 
            if (_cache.TryGetValue("ValidationKeys", out List<SecurityKey> validationKeys))
            {
                return validationKeys;
            }

            validationKeys = new List<SecurityKey>();
            var certificateVersions = await _keyVaultClient.GetCertificateVersionsAsync(_vault, _certificateName);

            foreach (var certificateItem in certificateVersions)
            {
                if (certificateItem.Attributes.Enabled.HasValue && certificateItem.Attributes.Enabled.Value)
                {
                    var certificateVersionBundle = await _keyVaultClient.GetCertificateAsync(certificateItem.Identifier.Identifier);
                    var certificateVersionSecurityKey = await GetSecurityKeyFromSecretAsync(_keyVaultClient, certificateVersionBundle.SecretIdentifier.Identifier).ConfigureAwait(false);

                    validationKeys.Add(certificateVersionSecurityKey);
                }
            }

            var options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddDays(1);
            _cache.Set("ValidationKeys", validationKeys, options);

            return validationKeys;
        }

        private async Task<SecurityKey> GetSecurityKeyFromSecretAsync(KeyVaultClient keyVaultClient, string secretIdentifier)
        {
            var certificatePrivateKeySecretBundle = await keyVaultClient.GetSecretAsync(secretIdentifier);
            var privateKeyBytes = Convert.FromBase64String(certificatePrivateKeySecretBundle.Value);
            var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);
            return new X509SecurityKey(certificateWithPrivateKey);
        }
    }
}