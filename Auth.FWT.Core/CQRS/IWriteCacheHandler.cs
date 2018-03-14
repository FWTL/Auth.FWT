using System;
using System.Threading.Tasks;

namespace Auth.FWT.Core.CQRS
{
    public interface IWriteCacheHandler<TQuery, TResult> : ICacheKey<TQuery>
    {
        Task Save(TQuery query, TResult result, TimeSpan? ttl = null);
    }
}