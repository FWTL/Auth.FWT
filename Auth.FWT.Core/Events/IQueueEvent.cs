using System.Threading.Tasks;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Core.Events
{
    public interface IEvent
    {
        Task Send(IServiceBus serviceBus);
    }
}