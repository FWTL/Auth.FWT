using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Configuration
{
    public class AzureSecretsVaultProvider : ConfigurationProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;

        public AzureSecretsVaultProvider(string baseUrl, string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _baseUrl = baseUrl;
        }

        public override void Load()
        {
            var parser = new AzureKeyVault(_baseUrl, _clientId, _clientSecret);
            Task<IDictionary<string, string>> t = parser.GetSecretsAsync();
            t.Wait();
            Data = t.Result;
        }
    }
}