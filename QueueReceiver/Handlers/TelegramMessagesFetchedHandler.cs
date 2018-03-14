using System.Data.Entity;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Events;
using NodaTime;
using StackExchange.Redis;

namespace QueueReceiver.Handlers
{
    public class TelegramMessagesFetchedHandler : IEventHandler<TelegramMessagesFetched>
    {
        private IDatabase _cache;
        private IClock _clock;
        private IEventDispatcher _eventDispatcher;
        private IUnitOfWork _unitOfWork;

        public TelegramMessagesFetchedHandler(IUnitOfWork unitOfWork, IClock clock, IDatabase cache, IEventDispatcher eventDispatcher)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
            _eventDispatcher = eventDispatcher;
            _cache = cache;
        }

        public async Task Execute(TelegramMessagesFetched @event)
        {
            var job = await _unitOfWork.TelegramJobRepository.Query().FirstOrDefaultAsync(tj => tj.JobId == @event.JobId);
            job.LastStatusUpdateDateUTC = _clock.UtcNow();
            job.Status = Auth.FWT.Core.Enums.Enum.TelegramJobStatus.Fetching;

            _unitOfWork.TelegramJobRepository.Update(job);
            await _unitOfWork.SaveChangesAsync();

            await _cache.StringIncrementAsync($"Fetching{job.JobId}", @event.FetchedCount);
            await _cache.StringSetAsync($"FetchingTotal{job.JobId}", @event.Total);

            await _eventDispatcher.Dispatch(new TelegramJobModified()
            {
                UserId = job.UserId,
                TelegramJobId = job.Id
            });
        }
    }
}