using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.API.Controllers.Events
{
    public class ReplaceValueInCache<TValue> : IEvent
    {
        private KeyValuePair<TValue> _pair;

        public ReplaceValueInCache(string key, TValue value)
        {
            _pair = new KeyValuePair<TValue>(key, value);
        }

        public async Task Send(IServiceBus client)
        {
            await client.SendToQueueAsync("redis", _pair);
        }
    }
}