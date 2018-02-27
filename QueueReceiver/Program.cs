using Microsoft.Azure.WebJobs;

namespace QueueReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jobHostConfiguration = new JobHostConfiguration
            {
                JobActivator = new AutofacJobActivator(IocConfig.RegisterDependencies())
            };

            var host = new JobHost(jobHostConfiguration);
            host.RunAndBlock();
        }
    }
}