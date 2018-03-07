using System.Data.Entity;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Events;
using NodaTime;

namespace QueueReceiver.Handlers
{
    public class TelegramMessagesFetchedHandler : IEventHandler<TelegramMessagesFetched>
    {
        private IClock _clock;
        private IUnitOfWork _unitOfWork;

        public TelegramMessagesFetchedHandler(IUnitOfWork unitOfWork, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Execute(TelegramMessagesFetched @event)
        {
            var job = await _unitOfWork.TelegramJobRepository.Query().FirstOrDefaultAsync(tj => tj.JobId == @event.JobId);
            job.LastStatusUpdateDateUTC = _clock.UtcNow();
            job.Status = Auth.FWT.Core.Enums.Enum.TelegramJobStatus.Processing;
            
            _unitOfWork.TelegramJobRepository.Update(job);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}