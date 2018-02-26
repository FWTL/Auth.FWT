using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;

namespace Auth.FWT.API.Controllers.Events
{
    public class ReplaceValueInCache<TValue> : IEvent
    {
        private string _key;
        private TValue _value;

        public ReplaceValueInCache(string key, TValue value)
        {
            _key = key;
            _value = value;
        }

        public async Task Send(IServiceBus client)
        {
            await client.SendToQueueAsync("redis", _key, _value);
        }
    }
}