using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Events;
using Auth.FWT.Events;
using StackExchange.Redis;

namespace QueueReceiver.Handlers
{
    public class CacheHandler : IEventHandler<TelegramJobModified>, IEventHandler<AllTelegramMessagesFetched>, IEventHandler<TelegramMessagesFetchingFailed>
    {
        private IDatabase _cache;
        private IServer _cacheServer;

        public CacheHandler(IServer cacheServer, IDatabase cache)
        {
            _cacheServer = cacheServer;
            _cache = cache;
        }

        public List<IEvent> Events { get; set; } = new List<IEvent>();

        public async Task Execute(TelegramMessagesFetchingFailed @event)
        {
            await _cache.KeyDeleteAsync($"Fetching{@event.JobId}");
            await _cache.KeyDeleteAsync($"FetchingTotal{@event.JobId}");
        }

        public async Task Execute(AllTelegramMessagesFetched @event)
        {
            await _cache.KeyDeleteAsync($"Fetching{@event.JobId}");
            await _cache.KeyDeleteAsync($"FetchingTotal{@event.JobId}");
        }

        public async Task Execute(TelegramJobModified @event)
        {
            var keys = _cacheServer.Keys(pattern: $"GetDashboard{@event.UserId}*", pageSize: 100).ToArray();
            await _cache.KeyDeleteAsync(keys);
        }
    }
}