﻿using System.Threading.Tasks;
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
        private IEventDispatcher _eventDispatcher;
        private IUnitOfWork _unitOfWork;

        public TelegramFetchingMessagesJobStartedHandler(IUnitOfWork unitOfWork, IClock clock, IEventDispatcher eventDispatcher)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
            _eventDispatcher = eventDispatcher;
        }

        public async Task Execute(TelegramFetchingMessagesJobStarted @event)
        {
            var newJob = new TelegramJob()
            {
                CreatedDateUTC = _clock.UtcNow(),
                LastStatusUpdateDateUTC = _clock.UtcNow(),
                JobId = @event.JobId,
                Status = Auth.FWT.Core.Enums.Enum.TelegramJobStatus.Started,
                UserId = @event.InvokedBy,
            };

            _unitOfWork.TelegramJobRepository.Insert(newJob);
            await _unitOfWork.SaveChangesAsync();

            await _eventDispatcher.Dispatch(new TelegramJobModified()
            {
                UserId = newJob.UserId,
                TelegramJobId = newJob.Id
            });
        }
    }
}