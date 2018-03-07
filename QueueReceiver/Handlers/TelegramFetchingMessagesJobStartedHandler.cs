using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Events;
using NodaTime;

namespace QueueReceiver.Handlers
{
    public class TelegramFetchingMessagesJobStartedHandler : IEventHandler<TelegramFetchingMessagesJobStarted>
    {
        private IClock _clock;
        private IUnitOfWork _unitOfWork;

        public TelegramFetchingMessagesJobStartedHandler(IUnitOfWork unitOfWork, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Execute(TelegramFetchingMessagesJobStarted @event)
        {
            _unitOfWork.TelegramJobRepository.Insert(new TelegramJob()
            {
                CreatedDateUTC = _clock.UtcNow(),
                LastStatusUpdateDateUTC = _clock.UtcNow(),
                JobId = @event.JobId,
                Status = Auth.FWT.Core.Enums.Enum.TelegramJobStatus.Started,
                UserId = @event.InvokedBy,
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}