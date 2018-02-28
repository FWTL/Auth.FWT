using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;
using static Auth.FWT.API.Controllers.Chat.GetUserChats;

namespace Auth.FWT.API.Controllers.Events
{
    public class UserChatsRefreshed : IEvent
    {
        public Query Query { get; set; }
        public List<Result> Result { get; set; }

        public UserChatsRefreshed(Query query, List<Result> result)
        {
            Result = result;
            Query = query;
        }

        public async Task Send(IServiceBus serviceBus)
        {
            await serviceBus.SendToQueueAsync("redis", this);
        }
    }
}