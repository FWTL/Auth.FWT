using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Events
{
    public class TelegramJobCreated : IEvent
    {
        public long JobId { get; set; }
        public int InvokedBy { get; set; }
        public ChannalType ChannalType { get; set; }
        public int ChannalId { get; set; }

        public Task Send(IServiceBus serviceBus)
        {
            serviceBus.SendToQueue("processing", this);
            return Task.FromResult(0);
        }
    }

    public enum ChannalType
    {
        User = 1,
        Chat = 2,
        Channal = 3,
    }
}