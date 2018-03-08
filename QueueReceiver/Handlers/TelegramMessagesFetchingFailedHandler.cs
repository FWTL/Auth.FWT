using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Events;
using NodaTime;

namespace QueueReceiver.Handlers
{
    public class TelegramMessagesFetchingFailedHandler : IEventHandler<TelegramMessagesFetchingFailed>
    {
        private IClock _clock;
        private IUnitOfWork _unitOfWork;

        public TelegramMessagesFetchingFailedHandler(IUnitOfWork unitOfWork, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public List<IEvent> Events { get; set; } = new List<IEvent>();

        public async Task Execute(TelegramMessagesFetchingFailed @event)
        {
            var job = await _unitOfWork.TelegramJobRepository.Query().FirstOrDefaultAsync(tj => tj.JobId == @event.JobId);
            job.LastStatusUpdateDateUTC = _clock.UtcNow();
            job.Status = Auth.FWT.Core.Enums.Enum.TelegramJobStatus.Error;

            _unitOfWork.TelegramJobRepository.Update(job);
            await _unitOfWork.SaveChangesAsync();

            Events.Add(new TelegramJobModified()
            {
                UserId = job.UserId,
                TelegramJobId = job.Id
            });
        }
    }
}