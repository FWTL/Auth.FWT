using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.API.CQRS;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Events;
using Hangfire;
using static Auth.FWT.API.Controllers.Job.Fetch.FetchMessages;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class StartImportingHistory
    {
        public class Handler :
            ICommandHandler<StartImportingUserChatHistory>,
            ICommandHandler<StartImportingChatHistory>,
            ICommandHandler<StartImportingChannalHistory>
        {
            private IServiceBus _serviceBus;

            private ITelegramClient _telegramClient;

            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession, IServiceBus serviceBus)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
                _serviceBus = serviceBus;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task Execute(StartImportingChannalHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Channal,
                    ChannalId = command.ChannelId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<HangfireCommandDispatcher>(gm => gm.Dispatch(new FetChannalMessages()
                {
                    ChannalId = command.ChannelId,
                    CurrentUserId = command.CurrentUserId,
                    JobId = jobId,
                    MaxId = int.MaxValue
                }, null));
            }

            public async Task Execute(StartImportingChatHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Chat,
                    ChannalId = command.ChatId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<HangfireCommandDispatcher>(gm => gm.Dispatch(new FetChatMessages()
                {
                    ChatId = command.ChatId,
                    CurrentUserId = command.CurrentUserId,
                    JobId = jobId,
                    MaxId = int.MaxValue
                }, null));
            }

            public async Task Execute(StartImportingUserChatHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.User,
                    ChannalId = command.UserId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<HangfireCommandDispatcher>(gm => gm.Dispatch(new FetchUserMessages()
                {
                    UserId = command.UserId,
                    CurrentUserId = command.CurrentUserId,
                    JobId = jobId,
                    MaxId = int.MaxValue
                }, null));
            }
        }

        public class StartImportingChannalHistory : ICommand
        {
            public int ChannelId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class StartImportingChatHistory : ICommand
        {
            public int ChatId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class StartImportingUserChatHistory : ICommand
        {
            public int CurrentUserId { get; set; }

            public string Title { get; internal set; }

            public int UserId { get; set; }
        }
    }
}