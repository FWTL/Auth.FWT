using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace QueueReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jobHostConfiguration = new JobHostConfiguration
            {
                JobActivator = new AutofacJobActivator(IocConfig.RegisterDependencies()),
            };

            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString = ConfigKeys.ServiceBus,
            };
            jobHostConfiguration.UseServiceBus(serviceBusConfig);

            var host = new JobHost(jobHostConfiguration);
            host.RunAndBlock();
        }
    }
}