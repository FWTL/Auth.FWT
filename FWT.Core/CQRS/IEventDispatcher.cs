using System.Threading.Tasks;

namespace FWT.Core.CQRS
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
