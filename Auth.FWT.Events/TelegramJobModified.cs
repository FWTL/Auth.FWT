using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.Events
{
    public class TelegramJobModified : IEvent
    {
        public long TelegramJobId { get; set; }
        public int UserId { get; set; }

        public Task Send(IServiceBus serviceBus)
        {
            throw new NotImplementedException();
        }
    }
}