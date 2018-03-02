using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Events;
using Autofac;

namespace Auth.FWT.API.CQRS
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IComponentContext _context;

        public EventDispatcher(IComponentContext context)
        {
            _context = context;
        }

        public async Task Dispatch<TEvent>(TEvent @event) where TEvent : IEvent
        {
            IEnumerable<IEventHandler<TEvent>> handlers = _context.Resolve<IEnumerable<IEventHandler<TEvent>>>().ToList();
            foreach (var handler in handlers)
            {
                await handler.Execute(@event);
            }
        }
    }
}