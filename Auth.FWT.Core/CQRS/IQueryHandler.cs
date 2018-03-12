using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;

namespace Auth.FWT.CQRS
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> Handle(TQuery query);

        List<IEvent> Events { get; }
    }
}