using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.KeyVault.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSetup
{
    internal class Program
    {
        private static async Task Dev()
        {
            Console.WriteLine("START");
            var options = new Options("auth2711dev");
            options.ASPNETCORE_ENVIRONMENT = "Staging";

            var azure = Util.Auth();
            try
            {
                var resourceGroupFactory = new AzureResourceGroup(azure, options);
                var redisFactory = new AzureRedis(azure, options);
                var vaultFactory = new AzureKeyVault(azure, options);
                var appPlanFactory = new AzureAppServices(azure, options);

                var resourceGroup = await resourceGroupFactory.CreateOrGetAsync(options.NAME, Region.EuropeNorth);
                var configVault = await vaultFactory.CreateOrGetAsync("config-" + options.NAME, resourceGroup, "650b5c4d-c28c-46f3-ae5d-397216471737");
                var plan = await appPlanFactory.CreateOrGetPlan(options.NAME, resourceGroup, PricingTier.BasicB1);

                await appPlanFactory.CreateOrGetWebApp("auth-" + options.NAME, resourceGroup, plan, new Dictionary<string, string>
                {
                    { "ASPNETCORE_ENVIRONMENT", options.ASPNETCORE_ENVIRONMENT },
                    { "AzureKeyVault:App:BaseUrl", $"https://config-{options.NAME}.vault.azure.net" },
                    { "AzureKeyVault:App:ClientId", options.READ_AD_APP_APPLICATIONID },
                    { "AzureKeyVault:App:SecretId", options.READ_AD_APP_SECRET }
                });

                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Auth-Clients-App-Secret", Util.CreateRandomPassword(20));
                await vaultFactory.AddKey($"config-{options.NAME}", "RsaKey");

                options.WriteToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
                options.WriteToFile();
            }
        }

        public static void Main()
        {
            var t = Dev();
            t.Wait();
        }
    }
}