using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace QueueReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new JobHostConfiguration
            {
                JobActivator = new AutofacJobActivator(IocConfig.RegisterDependencies()),
            };

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString = ConfigKeys.ServiceBus,
                MessageOptions = new Microsoft.ServiceBus.Messaging.OnMessageOptions()
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                }
            };
            config.UseServiceBus(serviceBusConfig);

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}