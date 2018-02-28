using System.Threading.Tasks;
using Auth.FWT.Core.Events;

namespace Auth.FWT.Core.CQRS
{
    public interface IEventDispatcher
    {
        Task Dispatch<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}