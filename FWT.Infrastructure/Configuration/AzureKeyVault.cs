using FWT.Core.Services.KeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Configuration
{
    public class AzureKeyVault : IAzureKeyVault
    {
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AzureKeyVault(string baseUrl, string clientId, string clientSecret)
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<RsaSecurityKey> GetRsaKeyAsync(string keyId)
        {
            var dict = new Dictionary<string, string>();
            using (var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback((authority, resource, scope)
            => GetTokenAsync(_clientId, _clientSecret, authority, resource, scope))))
            {
                KeyBundle key = await keyVaultClient.GetKeyAsync(keyId);
                return new RsaSecurityKey(key.Key.ToRSA());
            }
        }

        public async Task<IDictionary<string, string>> ParseAsync()
        {
            var dict = new Dictionary<string, string>();
            using (var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback((authority, resource, scope)
            => GetTokenAsync(_clientId, _clientSecret, authority, resource, scope))))
            {
                IPage<SecretItem> secrets = null;
                do
                {
                    secrets = await keyVaultClient.GetSecretsAsync(_baseUrl).ConfigureAwait(false);
                    foreach (SecretItem secret in secrets)
                    {
                        var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier).ConfigureAwait(false)).Value;
                        dict.Add(ParseKey(secret.Identifier.Name), value);
                    }
                }
                while (!string.IsNullOrWhiteSpace(secrets.NextPageLink));
            }

            return dict;
        }

        private static string ParseKey(string key)
        {
            return key.Replace("-", ":");
        }

        private static async Task<string> GetTokenAsync(string clientId, string clientSecret, string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}