using System.Threading.Tasks;
using Auth.FWT.CQRS;

namespace Auth.FWT.Core.CQRS
{
    public interface IReadCacheHandler<TQuery, TResult> : ICacheKey<TQuery>
    {
        Task<TResult> Read(TQuery query);
    }
}