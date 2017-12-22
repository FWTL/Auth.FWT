using StackExchange.Redis;

namespace Auth.FWT.Core.Services.Cache
{
    public interface IRedisClient
    {
        IDatabase Cache { get; }
    }
}
