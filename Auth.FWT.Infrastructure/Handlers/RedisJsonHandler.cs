using System;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.CQRS;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Auth.FWT.Infrastructure.Handlers
{
    public class RedisJsonHandler<TQuery, TResult> : IReadCacheHandler<TQuery, TResult>, IWriteCacheHandler<TQuery, TResult> where TQuery : IQuery where TResult : class
    {
        protected IDatabase _redis;

        public RedisJsonHandler(IDatabase redis)
        {
            _redis = redis;
        }

        public virtual async Task<TResult> Read(TQuery key)
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

        public async Task Save(TQuery query, TResult result, TimeSpan? ttl = null)
        {
            await _redis.StringSetAsync(KeyFn(query), JsonConvert.SerializeObject(result), ttl);
        }

        public Func<TQuery, string> KeyFn { get; set; }
    }
}