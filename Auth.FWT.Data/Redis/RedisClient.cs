using Auth.FWT.Core.Services.Cache;
using StackExchange.Redis;

namespace Auth.FWT.Data.Redis
{
    public class RedisClient : IRedisClient
    {
        public IDatabase Cache
        {
            get
            {
                return RedisConnector.Connection.GetDatabase();
            }
        }
    }
}
