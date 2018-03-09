using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.API.Controllers.Job;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Events;
using Hangfire;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class StartFeetchingHistory
    {
        public class Handler :
            ICommandHandler<StartFeetchingUserChatHistory>,
            ICommandHandler<StartFeetchingChatHistory>,
            ICommandHandler<StartFeetchingChannalHistory>
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

            public async Task Execute(StartFeetchingChannalHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Channal,
                    ChannalId = command.ChannelId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChannalHistory(command.CurrentUserId, command.ChannelId, int.MaxValue, jobId));
            }

            public async Task Execute(StartFeetchingChatHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Chat,
                    ChannalId = command.ChatId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChatHistory(command.CurrentUserId, command.ChatId, int.MaxValue, jobId));
            }

            public async Task Execute(StartFeetchingUserChatHistory command)
            {
                var jobId = Guid.NewGuid();

                await new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.User,
                    ChannalId = command.UserId,
                }.Send(_serviceBus);

                BackgroundJob.Enqueue<GetMessages>(gm => gm.UserChatHistory(command.CurrentUserId, command.UserId, int.MaxValue, jobId));
            }
        }

        public class StartFeetchingChannalHistory : ICommand
        {
            public int ChannelId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class StartFeetchingChatHistory : ICommand
        {
            public int ChatId { get; set; }

            public int CurrentUserId { get; set; }
        }

        public class StartFeetchingUserChatHistory : ICommand
        {
            public int CurrentUserId { get; set; }

            public string Title { get; internal set; }

            public int UserId { get; set; }
        }
    }
}
