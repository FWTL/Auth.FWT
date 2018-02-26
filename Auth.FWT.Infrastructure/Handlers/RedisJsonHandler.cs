using System;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.CQRS;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Auth.FWT.Infrastructure.Handlers
{
    public class RedisJsonHandler<TQuery, TResult> : ICachableHandler<TQuery, TResult> where TQuery : IQuery where TResult : class
    {
        private IDatabase _redis;

        public RedisJsonHandler(IDatabase redis)
        {
            _redis = redis;
        }

        public async Task<TResult> Read(TQuery key)
        {
            if (key == null)
            {
                throw new Exception("KeyFn not defined");
            }

            RedisValue redisValue = await _redis.StringGetAsync(KeyFn(key));
            if (redisValue.IsNull)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<TResult>(redisValue);
        }

        public Func<TQuery, string> KeyFn { get; set; }
    }
}