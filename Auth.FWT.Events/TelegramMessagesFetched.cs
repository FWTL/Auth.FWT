using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Events
{
    public class TelegramMessagesFetched : IEvent
    {
        public int FetchedCount { get; set; }
        public Guid JobId { get; set; }
        public int Total { get; set; }

        public Task Send(IServiceBus serviceBus)
        {
            serviceBus.SendToQueue("processing", this);
            return Task.CompletedTask;
        }
    }
}