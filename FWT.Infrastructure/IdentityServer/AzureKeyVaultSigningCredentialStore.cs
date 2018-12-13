using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;


namespace FWTL.Infrastructure.IdentityServer
{
  
    public class AzureKeyVaultSigningCredentialStore : ISigningCredentialStore
    {
        private readonly IMemoryCache _cache;

        private readonly string _certificateName;

        private readonly KeyVaultClient _keyVaultClient;

        private readonly string _vault;

        public AzureKeyVaultSigningCredentialStore(IMemoryCache memoryCache, KeyVaultClient keyVaultClient, string vault, string certificateName)
        {
            _cache = memoryCache;
            _keyVaultClient = keyVaultClient;
            _vault = vault;
            _certificateName = certificateName;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            if (_cache.TryGetValue("SigningCredentials", out SigningCredentials signingCredentials))
            {
                return signingCredentials;
            }
            var certificateBundle = await _keyVaultClient.GetCertificateAsync(_vault, _certificateName);

            var certificatePrivateKeySecretBundle = await _keyVaultClient.GetSecretAsync(certificateBundle.SecretIdentifier.Identifier);
            var privateKeyBytes = Convert.FromBase64String(certificatePrivateKeySecretBundle.Value);
            var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);

            signingCredentials = new SigningCredentials(new X509SecurityKey(certificateWithPrivateKey), SecurityAlgorithms.RsaSha256);

            var options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddDays(1);
            _cache.Set("SigningCredentials", signingCredentials, options);

            return signingCredentials;
        }
    }
}
