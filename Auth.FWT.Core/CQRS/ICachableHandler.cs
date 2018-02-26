using System.Threading.Tasks;
using Auth.FWT.CQRS;

namespace Auth.FWT.Core.CQRS
{
    public interface ICachableHandler<TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> Read(TQuery query);
    }
}