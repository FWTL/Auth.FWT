using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Events
{
    public class AllTelegramMessagesFetched : IEvent
    {
        public Guid JobId { get; set; }
        public int InvokedBy { get; set; }

        public Task Send(IServiceBus serviceBus)
        {
            serviceBus.SendToQueue("processing", this);
            return Task.CompletedTask;
        }
    }
}