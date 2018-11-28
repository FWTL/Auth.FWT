using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.KeyVault.Fluent;
using Microsoft.Azure.Management.KeyVault.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using System;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class AzureKeyVault
    {
        private IAzure _azure;
        private Options _options;
        private KeyVaultClient _keyVaultClient;

        public AzureKeyVault(IAzure azure, Options options)
        {
            _azure = azure;
            _options = options;
            _keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback((authority, resource, scope) => Util.GetToken(options.AD_APP_APPLICATIONID, options.AD_APP_SECRET, authority, resource, scope)));
        }

        public async Task<IVault> CreateOrGetAsync(string name, IResourceGroup group, string principalObjectId, params SecretPermissions[] secretPermissions)
        {
            Console.WriteLine($"AzureKeyVault.CreateOrGetAsync {name}");

            var vault = _azure.Vaults.GetByResourceGroup(group.Name, name);
            if (vault != null)
            {
                return vault;
            }

            return await _azure.Vaults.Define(name)
                .WithRegion(group.Region)
                .WithExistingResourceGroup(group)
                .DefineAccessPolicy()
                    .ForServicePrincipal("AG Setup")
                    .AllowSecretPermissions(SecretPermissions.Get, SecretPermissions.List, SecretPermissions.Set)
                    .AllowKeyPermissions(KeyPermissions.Create, KeyPermissions.Get)
                    .Attach()
                .DefineAccessPolicy()
                    .ForObjectId(principalObjectId)
                    .AllowSecretPermissions(SecretPermissions.Get)
                    .AllowKeyPermissions(KeyPermissions.Get)
                    .Attach()
                .CreateAsync();
        }

        public async Task AddSecretToVault(string keyVaultName, string key, string value)
        {
            Console.WriteLine($"AzureKeyVault.AddSecretToVault {key}");

            _options.AddSettings(key, value);
            await _keyVaultClient.SetSecretAsync($"https://{keyVaultName}.vault.azure.net", key, value);
        }

        public async Task AddCertificate(string keyVaultName, string key)
        {
            Console.WriteLine($"AzureKeyVault.AddCertificate {key}");
            var certificate = await _keyVaultClient.GetCertificateAsync(key);
            if (certificate == null)
            {
                await _keyVaultClient.CreateCertificateAsync($"https://{keyVaultName}.vault.azure.net", key);
            }
        }

        public async Task AddKey(string keyVaultName, string key)
        {
            Console.WriteLine($"AzureKeyVault.AddKey {key}");
            try
            {
                var rsaKey = await _keyVaultClient.GetKeyAsync(key);
                if (rsaKey != null)
                {
                    return;
                }
            }
            catch { }

            var keyBundle = GetKeyBundle();
            await _keyVaultClient.CreateKeyAsync($"https://{keyVaultName}.vault.azure.net", key, new NewKeyParameters()
            {
                Kty = keyBundle.Key.Kty,
                Attributes = keyBundle.Attributes
            });
        }

        public KeyBundle GetKeyBundle()
        {
            var defaultKeyBundle = new KeyBundle
            {
                Key = new JsonWebKey()
                {
                    Kty = JsonWebKeyType.Rsa,
                },
                Attributes = new KeyAttributes()
                {
                    Enabled = true,
                },
            };

            return defaultKeyBundle;
        }
    }
}