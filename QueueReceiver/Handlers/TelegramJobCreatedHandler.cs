using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Events;
using NodaTime;

namespace QueueReceiver.Handlers
{
    public class TelegramJobCreatedHandler : IEventHandler<TelegramJobCreated>
    {
        private IClock _clock;
        private IEventDispatcher _eventDispatcher;
        private IUnitOfWork _unitOfWork;

        public TelegramJobCreatedHandler()
        {
        }

        public async Task Execute(TelegramJobCreated @event)
        {
        }
    }
}