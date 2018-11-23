using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace FWT.Infrastructure.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "FWT.API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "app",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AccessTokenLifetime = 3600 * 24 * 30,

                    ClientSecrets =
                    {
                        new Secret(configuration["Auth:Clients:App:Secret"].Sha256())
                    },

                    AllowedScopes =
                    {
                        "api"
                    },
                },

                new Client
                {
                    ClientId = "swagger",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    Claims = new List<Claim>() { new Claim("PhoneHash", "123") },
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        "api"
                    },
                },
            };
        }
    }
}