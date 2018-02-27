using System.Threading.Tasks;
using Auth.FWT.CQRS;

namespace Auth.FWT.Core.CQRS
{
    public interface IReadCacheHandler<TItem, TResult>
    {
        Task<TResult> Read(TItem query);
    }
}