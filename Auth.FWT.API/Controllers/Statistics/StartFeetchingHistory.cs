using System;
using System.Threading.Tasks;
using Auth.FWT.API.Controllers.Jobs;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Events;
using Hangfire;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class StartFeetchingHistory
    {
        public class StartFeetchingUserChatHistory : ICommand
        {
            public int CurrentUserId { get; set; }
            public int UserId { get; set; }
        }

        public class StartFeetchingChatHistory : ICommand
        {
            public int CurrentUserId { get; set; }
            public int ChatId { get; set; }
        }

        public class StartFeetchingChannalHistory : ICommand
        {
            public int CurrentUserId { get; set; }
            public int ChannelId { get; set; }
        }

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

            public async Task Execute(StartFeetchingChannalHistory command)
            {
                var jobId = Guid.NewGuid();
                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChannalHistory(command.CurrentUserId, command.ChannelId, int.MaxValue, jobId));

                await _serviceBus.SendToQueueAsync("processing", new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Channal,
                    ChannalId = command.ChannelId
                });
            }

            public async Task Execute(StartFeetchingChatHistory command)
            {
                var jobId = Guid.NewGuid();
                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChannalHistory(command.CurrentUserId, command.ChatId, int.MaxValue, jobId));

                await _serviceBus.SendToQueueAsync("processing", new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.Chat,
                    ChannalId = command.ChatId
                });
            }

            public async Task Execute(StartFeetchingUserChatHistory command)
            {
                var jobId = Guid.NewGuid();
                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChannalHistory(command.CurrentUserId, command.UserId, int.MaxValue, jobId));

                await _serviceBus.SendToQueueAsync("processing", new TelegramFetchingMessagesJobStarted()
                {
                    JobId = jobId,
                    InvokedBy = command.CurrentUserId,
                    ChannalType = ChannalType.User,
                    ChannalId = command.UserId
                });
            }
        }
    }
}