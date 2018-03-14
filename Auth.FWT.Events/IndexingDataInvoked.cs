using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Events
{
    public class IndexingDataInvoked : IEvent
    {
        public long TelegramJobId { get; set; }

        public async Task Send(IServiceBus serviceBus)
        {
            await serviceBus.SendToQueueAsync("processing", this);
        }
    }
}