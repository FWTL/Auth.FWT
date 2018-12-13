using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace FWTL.Infrastructure.IdentityServer
{
    

    public static class IdentityServerAzureKeyVaultConfigurationExtensions
    {
        public static IIdentityServerBuilder AddSigningCredentialFromAzureKeyVault(this IIdentityServerBuilder builder, string vault, string certificateName)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var authenticationCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            var keyVaultClient = new KeyVaultClient(authenticationCallback);

            builder.Services.AddMemoryCache();

            var sp = builder.Services.BuildServiceProvider();
            builder.Services.AddSingleton<ISigningCredentialStore>(new AzureKeyVaultSigningCredentialStore(sp.GetService<IMemoryCache>(), keyVaultClient, vault, certificateName));
            builder.Services.AddSingleton<IValidationKeysStore>(new AzureKeyVaultValidationKeysStore(sp.GetService<IMemoryCache>(), keyVaultClient, vault, certificateName));

            return builder;
        }

        public static IIdentityServerBuilder AddSigningCredentialFromAzureKeyVault(this IIdentityServerBuilder identityServerbuilder, string vault, string clientId, string clientSecret, string certificateName, double signingKeyRolloverPeriod)
        {
            KeyVaultClient.AuthenticationCallback authenticationCallback = (authority, resource, scope) => GetTokenFromClientSecret(authority, resource, clientId, clientSecret);
            var keyVaultClient = new KeyVaultClient(authenticationCallback);

            identityServerbuilder.Services.AddMemoryCache();

            var sp = identityServerbuilder.Services.BuildServiceProvider();
            identityServerbuilder.Services.AddSingleton<ISigningCredentialStore>(new AzureKeyVaultSigningCredentialStore(sp.GetService<IMemoryCache>(), keyVaultClient, vault, certificateName));
            identityServerbuilder.Services.AddSingleton<IValidationKeysStore>(new AzureKeyVaultValidationKeysStore(sp.GetService<IMemoryCache>(), keyVaultClient, vault, certificateName));

            return identityServerbuilder;
        }

        private static async Task<string> GetTokenFromClientSecret(string authority, string resource, string clientId, string clientSecret)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            return result.AccessToken;
        }
    }
}
