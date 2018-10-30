using Microsoft.Extensions.Configuration;

namespace FWT.Infrastructure.Configuration
{
    public class AzureSecretsVaultSource : IConfigurationSource
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;

        public AzureSecretsVaultSource(string baseUrl, string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _baseUrl = baseUrl;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureSecretsVaultProvider(_baseUrl, _clientId, _clientSecret);
        }
    }
}