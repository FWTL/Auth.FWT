using System.Threading.Tasks;

namespace Auth.FWT.CQRS
{
    public interface IQueryDispatcher
    {
        Task<TResult> Dispatch<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }
}
