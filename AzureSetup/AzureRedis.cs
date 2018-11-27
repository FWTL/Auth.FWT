using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Redis.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using System;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class AzureRedis
    {
        private IAzure _azure;
        private Options _options;

        public AzureRedis(IAzure azure, Options options)
        {
            _azure = azure;
            _options = options;
        }

        public async Task<IRedisCache> CreateOrGetAsync(string name, IResourceGroup group)
        {
            Console.WriteLine($"AzureRedis.CreateOrGetAsync {name}");

            IRedisCache redis = _azure.RedisCaches.GetByResourceGroup(group.Name, name);
            if (redis != null)
            {
                return redis;
            }

            return await _azure.RedisCaches.Define(name)
               .WithRegion(group.Region)
               .WithExistingResourceGroup(group)
               .WithBasicSku()
               .WithNonSslPort()
               .CreateAsync();
        }
    }
}