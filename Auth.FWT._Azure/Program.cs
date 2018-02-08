using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Management.Automation;
using InquirerCS;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Auth.FWT._Azure
{
    public class Answers
    {
        public IAppServicePlan AppServicePlan { get; internal set; }
        public IResourceGroup ResourceGroup { get; set; }
    }

    internal class Program
    {
        private static IAzure _azure;
        private static dynamic _answers = new ExpandoObject();
        private static Answers Answers = new Answers();

        private static void Main(string[] args)
        {
            CreateAzureAuthPropertiesInRegistry();
            CreateOrUseExistingResourceGroup();
            CreateOrUseExistingPlan();
            CreateWebApp();
            Inquirer.Go();
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
                    .WithPricingTier(PricingTier.FreeF1)
                    .WithOperatingSystem(Microsoft.Azure.Management.AppService.Fluent.OperatingSystem.Windows)
                .Create();
            });
        }

        private static void CreateWebApp()
        {
            Inquirer.Prompt(Question.Input("Web app name").WithConfirmation()).Then(webAppName =>
            {
                _azure.WebApps.Define(webAppName)
                    .WithExistingWindowsPlan(Answers.AppServicePlan)
                    .WithExistingResourceGroup(Answers.ResourceGroup)
                .Create();
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
            Inquirer.Prompt(Question.List("Region", Region.Values.ToList()).WithConfirmation().WithDefaultValue(region => region == Region.EuropeNorth)).Then(region =>
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