using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.API.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.CQRS;
using Auth.FWT.Events;
using Hangfire;
using NodaTime;
using static Auth.FWT.API.Controllers.Job.Fetch.ImportMessages;

namespace Auth.FWT.API.Controllers.Job.Import
{
    public class BeginImportingMessages
    {
        public class Handler :
            ICommandHandler<ImportUserChatHistory>,
            ICommandHandler<ImportChatHistory>,
            ICommandHandler<ImportChannalHistory>
        {
            private IClock _clock;
            private IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork, IClock clock)
            {
                _unitOfWork = unitOfWork;
                _clock = clock;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task Execute(ImportChannalHistory command)
            {
                var newJob = new TelegramJob()
                {
                    CreatedDateUTC = _clock.UtcNow(),
                    LastStatusUpdateDateUTC = _clock.UtcNow(),
                    Status = Core.Enums.Enum.TelegramJobStatus.Started,
                    UserId = command.CurrentUserId,
                };

                _unitOfWork.TelegramJobRepository.Insert(newJob);
                await _unitOfWork.SaveChangesAsync();

                Events.Add(new TelegramJobCreated()
                {
                    JobId = newJob.Id,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Channal,
                    ChannalId = command.ChannelId,
                });
            }

            public async Task Execute(ImportChatHistory command)
            {
                var newJob = new TelegramJob()
                {
                    CreatedDateUTC = _clock.UtcNow(),
                    LastStatusUpdateDateUTC = _clock.UtcNow(),
                    Status = Core.Enums.Enum.TelegramJobStatus.Started,
                    UserId = command.CurrentUserId,
                };

                _unitOfWork.TelegramJobRepository.Insert(newJob);
                await _unitOfWork.SaveChangesAsync();

                Events.Add(new TelegramJobCreated()
                {
                    JobId = newJob.Id,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Chat,
                    ChannalId = command.ChatId,
                });
            }

            public async Task Execute(ImportUserChatHistory command)
            {
                var newJob = new TelegramJob()
                {
                    CreatedDateUTC = _clock.UtcNow(),
                    LastStatusUpdateDateUTC = _clock.UtcNow(),
                    Status = Core.Enums.Enum.TelegramJobStatus.Started,
                    UserId = command.CurrentUserId,
                };

                _unitOfWork.TelegramJobRepository.Insert(newJob);
                await _unitOfWork.SaveChangesAsync();

                Events.Add(new TelegramJobCreated()
                {
                    JobId = newJob.Id,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.User,
                    ChannalId = command.UserId,
                });
            }
        }

        public class ImportChannalHistory : ICommand
        {
            public int ChannelId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class ImportChatHistory : ICommand
        {
            public int ChatId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class ImportUserChatHistory : ICommand
        {
            public int CurrentUserId { get; set; }

            public string Title { get; internal set; }

            public int UserId { get; set; }
        }
    }
}