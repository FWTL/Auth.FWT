using System;
using System.Threading.Tasks;
using Auth.FWT.API.Controllers.Events;
using Auth.FWT.Core.CQRS;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace QueueReceiver
{
    public class RedisHandler : IEventHandler<UserChatsRefreshed>
    {
        private IDatabase _cache;

        public RedisHandler(IDatabase cache)
        {
            _cache = cache;
        }

        public async Task Execute(UserChatsRefreshed @event)
        {
            await Save(query => { return "GetUserChats" + query.Userid; }, @event.Query, @event.Result, TimeSpan.FromDays(2));
        }

        public virtual async Task Save<TQuery, TResult>(Func<TQuery, string> KeyFn, TQuery query, TResult result, TimeSpan? ttl = null)
        {
            await _cache.StringSetAsync(KeyFn(query), JsonConvert.SerializeObject(result), ttl);
        }
    }
}