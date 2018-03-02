using Autofac;
using Microsoft.Azure.WebJobs.Host;

namespace QueueReceiver
{
    public class AutofacJobActivator : IJobActivator
    {
        private readonly IContainer _container;

        public AutofacJobActivator(IContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}