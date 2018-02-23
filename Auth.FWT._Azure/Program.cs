using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using InquirerCS;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent;

namespace Auth.FWT._Azure
{
    public class Answers
    {
        public IAppServicePlan AppServicePlan { get; internal set; }
        public IResourceGroup ResourceGroup { get; set; }

        public ISqlServer SQLServer { get; set; }
    }

    internal class Program
    {
        private static IAzure _azure;
        private static Answers Answers = new Answers();

        private static void Main(string[] args)
        {
            CreateAzureAuthPropertiesInRegistry();
            Menu();
            Inquirer.Go();
        }

        private static void Menu()
        {
            Inquirer.Prompt(Question.Menu("")
            .AddOption("Create Web App", () =>
            {
                CreateOrUseExistingResourceGroup();
                CreateOrUseExistingPlan();
                CreateWebApp();
            })
            .AddOption("Create SQL Db", () =>
            {
                CreateOrUseExistingResourceGroup();
                CreateOrUseSQLServer();
            }));
        }

        private static void CreateDb()
        {
            var list = Answers.SQLServer.Databases.List();
            if (list.Any(db => db.Name != "AuthFWT"))
            {
                Answers.SQLServer.Databases.Define("AuthFWT").WithMaxSizeBytes(500000000);
            }
        }

        private static void CreateOrUseSQLServer()
        {
            Inquirer.Prompt(Question.Menu("Create or use existing SQL Server")
               .AddOption("Create", () => CreateSQLServer())
               .AddOption("Use existing", () => UseExistingSQLServer()));
        }

        private static void UseExistingSQLServer()
        {
            var sqlServers = _azure.SqlServers.ListByResourceGroup(Answers.ResourceGroup.Name);
            Inquirer.Prompt(Question.List("SQLServer", sqlServers).WithConvertToString(s => s.Name)).Bind(() => Answers.SQLServer)
            .After(() =>
            {
                CreateDb();
            });
        }

        private static void CreateSQLServer()
        {
            var login = string.Empty;
            var password = string.Empty;
            var sqlDbName = "devsql" + DateTime.UtcNow.Ticks;

            Inquirer.Prompt(Question.Input("DbName").WithDefaultValue(sqlDbName)).Bind(() => sqlDbName);
            Inquirer.Prompt(Question.Input("Login")).Bind(() => login);
            Inquirer.Prompt(Question.Password("Password").WithConfirmation()).Bind(() => password)
            .After(() =>
            {
                try
                {
                    Answers.SQLServer = _azure.SqlServers.Define(sqlDbName)
                        .WithRegion(Answers.ResourceGroup.Region)
                        .WithExistingResourceGroup(Answers.ResourceGroup)
                        .WithAdministratorLogin(login)
                        .WithAdministratorPassword(password)
                        .WithNewFirewallRule("0.0.0.0", "255.255.255.255")
                        .Create();
                    CreateDb();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                    CreateSQLServer();
                }
            });
        }

        private static void CreateOrUseExistingPlan()
        {
            Inquirer.Prompt(Question.Menu("Create or use existing plan")
                .AddOption("Create", () => CreatePlan())
                .AddOption("Use existing", () => UseExistingPlan()));
        }

        private static void UseExistingPlan()
        {
            Inquirer.Prompt(() =>
            {
                List<IAppServicePlan> servicePlans = _azure.AppServices.AppServicePlans.ListByResourceGroup(Answers.ResourceGroup.Name).ToList();
                return Question.List("Choose", servicePlans).WithConfirmation().WithConvertToString(item => item.Name);
            }).Then(answer =>
            {
                Answers.AppServicePlan = answer;
            });
        }

        private static void CreatePlan()
        {
            Inquirer.Prompt(Question.Input("Name for plan").WithConfirmation().WithDefaultValue("Dev")).Then(answer =>
            {
                Answers.AppServicePlan = _azure.AppServices.AppServicePlans.Define(answer)
                    .WithRegion(Answers.ResourceGroup.Region.Name)
                    .WithExistingResourceGroup(Answers.ResourceGroup)
                    .WithPricingTier(PricingTier.SharedD1)
                    .WithOperatingSystem(Microsoft.Azure.Management.AppService.Fluent.OperatingSystem.Windows)
                .Create();
            });
        }

        private static void CreateWebApp()
        {
            Inquirer.Prompt(Question.Input("Web app name").WithConfirmation().WithDefaultValue("devauthfwt" + DateTime.Now.Ticks)).Then(webAppName =>
            {
                try
                {
                    _azure.WebApps.Define(webAppName)
                        .WithExistingWindowsPlan(Answers.AppServicePlan)
                        .WithExistingResourceGroup(Answers.ResourceGroup)
                        .DefineHostnameBinding()
                            .WithThirdPartyDomain("agme.info")
                            .WithSubDomain("dev.auth.fwt")
                            .WithDnsRecordType(Microsoft.Azure.Management.AppService.Fluent.Models.CustomHostNameDnsRecordType.CName)
                            .Attach()
                    .Create();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                    CreateWebApp();
                }
            });
        }

        private static void CreateAzureAuthPropertiesInRegistry()
        {
            using (PowerShell powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript($@"[Environment]::SetEnvironmentVariable('AZURE_AUTH_LOCATION', '{AppDomain.CurrentDomain.BaseDirectory}azureauth.properties', 'User')");
                Collection<PSObject> output = powerShellInstance.Invoke();
            }
            Auth();
        }

        private static void Auth()
        {
            var credentials = SdkContext.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION", EnvironmentVariableTarget.User));
            _azure = Azure.Configure().WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic).Authenticate(credentials).WithDefaultSubscription();
        }

        private static void CreateOrUseExistingResourceGroup()
        {
            Inquirer.Prompt(Question.Menu("Create or use existing resource group")
                .AddOption("Create", () => CreateResourceGroup())
                .AddOption("Use existing", () => UseExistingResourceGroup()));
        }

        private static void CreateResourceGroup()
        {
            string resourceGroupName = null;

            Inquirer.Prompt(Question.Input("Group Name").WithConfirmation()).Bind(() => resourceGroupName);
            Inquirer.Prompt(Question.List("Region", Region.Values.ToList()).WithConfirmation().Page(10).WithDefaultValue(region => region == Region.EuropeNorth)).Then(region =>
            {
                Answers.ResourceGroup = _azure.ResourceGroups.Define(resourceGroupName).WithRegion(region).Create();
            });
        }

        private static void UseExistingResourceGroup()
        {
            Inquirer.Prompt(() =>
            {
                var groups = _azure.ResourceGroups.List();
                return Question.List("Resource Group", groups).WithConfirmation().WithConvertToString(x => x.Name);
            }).Bind(() => Answers.ResourceGroup);
        }
    }
}