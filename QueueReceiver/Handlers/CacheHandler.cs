using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.API.Controllers.Events;
using Auth.FWT.Core.CQRS;
using static Auth.FWT.API.Controllers.Chat.GetUserChats;

namespace QueueReceiver
{
    public class UserChatsRefreshedHandler : IEventHandler<UserChatsRefreshed>
    {
        private IWriteCacheHandler<Query, List<Result>> _writer;

        public UserChatsRefreshedHandler(IWriteCacheHandler<Query, List<Result>> writer)
        {
            _writer = writer;
        }

        public async Task Execute(UserChatsRefreshed @event)
        {
            _writer.KeyFn = query => { return "GetUserChats" + query.Userid; };
            await _writer.Save(@event.Query, @event.Result, TimeSpan.FromDays(2));
        }
    }
}