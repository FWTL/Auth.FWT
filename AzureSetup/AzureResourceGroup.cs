using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class AzureResourceGroup
    {
        public AzureResourceGroup(IAzure azure, Options options)
        {
            _azure = azure;
            _options = options;
        }

        public IAzure _azure;
        private Options _options;

        public async Task<IResourceGroup> CreateOrGetAsync(string name, Region region)
        {
            Console.WriteLine($"AzureResourceGroup.CreateOrGetAsync {name}");

            try
            {
                return _azure.ResourceGroups.GetByName(name);
            }
            catch { }

            return await _azure.ResourceGroups.Define(name).WithRegion(region).CreateAsync();
        }

        public async Task DeleteAsync(string name)
        {
            await _azure.ResourceGroups.DeleteByNameAsync(name);
        }
    }
}