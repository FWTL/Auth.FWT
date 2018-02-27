using System;
using System.Threading.Tasks;

namespace Auth.FWT.Core.CQRS
{
    public interface IWriteCacheHandler<TItem, TResult>
    {
        Task Save(TItem query, TResult result, TimeSpan? ttl = null);
    }
}