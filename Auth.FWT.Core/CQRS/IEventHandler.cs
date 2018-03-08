using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;

namespace Auth.FWT.Core.CQRS
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        List<IEvent> Events { get; }

        Task Execute(TEvent @event);
    }
}