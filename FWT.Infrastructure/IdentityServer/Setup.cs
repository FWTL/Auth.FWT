using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace FWT.Infrastructure.IdentityServer
{
    public class Setup
    {
        public static IEnumerable<Client> GetClients()
        {
            var appClient = new Client
            {
                ClientId = "app",
                ClientName = "JavaScript Client",

                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowAccessTokensViaBrowser = true,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    "api",
                },
            };

            return new List<Client>() { appClient };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var fwt = new ApiResource("api", "FWT.API");
            return new List<ApiResource>() { fwt };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var identityResources = new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResource( name: "custom.profile", displayName: "profile", claimTypes: new[] { "name", "userPhoneHashId" })
            };

            return identityResources;
        }
    }
}