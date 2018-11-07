using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.KeyVault.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSetup
{
    internal class Program
    {
        private static async Task Dev()
        {
            var options = new Options("d3108dev");
            var azure = Util.Auth();
            try
            {
                var resourceGroupFactory = new AzureResourceGroup(azure, options);
                var sqlServerFactory = new AzureSql(azure, options);
                var vaultFactory = new AzureKeyVault(azure, options);
                var appPlanFactory = new AzureAppServices(azure, options);

                var resourceGroup = await resourceGroupFactory.CreateOrGetAsync(options.NAME, Region.EuropeNorth);
                var sqlServer = await sqlServerFactory.CreateOrGetServerAsync(options.NAME, resourceGroup, "SQL_PASSWORD");

                var configVault = await vaultFactory.CreateOrGetAsync("config-" + options.NAME, resourceGroup, "SIPAzureSetup", SecretPermissions.Get, SecretPermissions.Set, SecretPermissions.List);
                var plan = await appPlanFactory.CreateOrGetPlan(options.NAME, resourceGroup, PricingTier.BasicB1);

                var hangfireDatabaseName = "Hangfire";
                var hangfireDatabase = await sqlServerFactory.CreateOrGetDatabase(hangfireDatabaseName, sqlServer, SqlDatabaseBasicStorage.Max500Mb);
                await sqlServerFactory.CreateDbUser(sqlServer, hangfireDatabase, options.AddSettings("SQL_Hangfire_PASSWORD", Util.CreateRandomPassword(20)));

                await appPlanFactory.CreateOrGetWebApp("scheduler-" + options.NAME, resourceGroup, plan, new Dictionary<string, string>
                {
                    { "ASPNETCORE_ENVIRONMENT", options.ASPNETCORE_ENVIRONMENT },
                    { "AzureKeyVault:App:BaseUrl", $"https://config-{options.NAME}.vault.azure.net" },
                    { "AzureKeyVault:App:ClientId", options.AD_APP_APPLICATIONID },
                    { "AzureKeyVault:App:SecretId", options.AD_APP_SECRET }
                });

                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Pim-Sql-Url", "138.103.2.3");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Pim-Sql-Port", "1433");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Pim-Sql-Catalog", "DP_PIM_Test");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Pim-Sql-User", "Softserve");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Pim-Sql-Password", "TNNas7L%ashGheTd");

                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Hangfire-Sql-Url", $"{options.NAME}.database.windows.net");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Hangfire-Sql-Port", "1433");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Hangfire-Sql-Catalog", hangfireDatabase.Name);
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Hangfire-Sql-User", $"{hangfireDatabase.Name}_login");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Hangfire-Sql-Password", options.GetKeyValue("SQL_Hangfire_PASSWORD"));

                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Commerce-Auth-ClientId", "integrations");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Commerce-Auth-Secret", "secret");
                await vaultFactory.AddSecretToVault($"config-{options.NAME}", "Commerce-Auth-Endpoint", "https://localhost:5050/");

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