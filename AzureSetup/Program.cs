using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;

namespace AzureSetup
{
    internal class Program
    {
        private static async Task Dev()
        {
            Console.WriteLine("START");
            var options = new Options("d0711dev");
            var azure = Util.Auth();
            try
            {
                var resourceGroupFactory = new AzureResourceGroup(azure, options);
                var redisFactory = new AzureRedis(azure, options);

                var resourceGroup = await resourceGroupFactory.CreateOrGetAsync(options.NAME, Region.EuropeNorth);
                var redis = await redisFactory.CreateOrGetAsync(options.NAME, resourceGroup);

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