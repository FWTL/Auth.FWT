using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class AzureAppServices
    {
        private IAzure _azure;
        private Options _options;

        public AzureAppServices(IAzure azure, Options options)
        {
            _azure = azure;
            _options = options;
        }

        public async Task<IAppServicePlan> CreateOrGetPlan(string name, IResourceGroup group, PricingTier tier)
        {
            Console.WriteLine($"AzureAppServices.CreateOrGetPlan {name}");

            var plan = _azure.AppServices.AppServicePlans.GetByResourceGroup(group.Name, name);
            if (plan != null)
            {
                return plan;
            }

            return await _azure.AppServices.AppServicePlans.Define(name)
               .WithRegion(group.Region)
               .WithExistingResourceGroup(group)
               .WithPricingTier(tier)
               .WithOperatingSystem(Microsoft.Azure.Management.AppService.Fluent.OperatingSystem.Windows)
           .CreateAsync();
        }

        public async Task<IWebApp> CreateOrGetWebApp(string name, IResourceGroup group, IAppServicePlan plan, Dictionary<string, string> settings = null)
        {
            Console.WriteLine($"AzureAppServices.CreateOrGetWebApp {name}");

            var webApp = _azure.AppServices.WebApps.GetByResourceGroup(group.Name, name);

            if (settings == null)
            {
                settings = new Dictionary<string, string>();
            }

            foreach (var setting in settings)
            {
                _options.AddSettings(setting.Key, setting.Value);
            }

            if (webApp != null)
            {
                return webApp;
            }

            return await _azure.WebApps.Define(name)
              .WithExistingWindowsPlan(plan)
              .WithExistingResourceGroup(group)
              .WithAppSettings(settings)
            .CreateAsync();
        }
    }
}