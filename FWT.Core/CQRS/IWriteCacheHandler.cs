using System.Threading.Tasks;

namespace FWT.Core.CQRS
{
    public interface IWriteCacheHandler<TQuery, TResult> : ICacheKey<TQuery>
    {
        Task WriteAsync(TQuery query, TResult result);
    }
}
