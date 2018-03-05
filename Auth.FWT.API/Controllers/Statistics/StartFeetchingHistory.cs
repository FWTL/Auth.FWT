using System.Threading.Tasks;
using Auth.FWT.API.Controllers.Jobs;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Hangfire;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class StartFeetchingHistory
    {
        public class StartFeetchingUserChatHistory : ICommand
        {
            public int CurrentuserId { get; set; }
            public int UserId { get; set; }
        }

        public class StartFeetchingChatHistory : ICommand
        {
            public int CurrentuserId { get; set; }
            public int ChatId { get; set; }
        }

        public class StartFeetchingChannalHistory : ICommand
        {
            public int CurrentuserId { get; set; }
            public int ChannelId { get; set; }
        }

        public class Handler :
            ICommandHandler<StartFeetchingUserChatHistory>,
            ICommandHandler<StartFeetchingChatHistory>,
            ICommandHandler<StartFeetchingChannalHistory>
        {
            private ITelegramClient _telegramClient;
            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
            }

            public Task Execute(StartFeetchingChannalHistory command)
            {
                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChannalHistory(command.CurrentuserId, command.ChannelId, int.MaxValue));
                return Task.FromResult(0);
            }

            public Task Execute(StartFeetchingChatHistory command)
            {
                BackgroundJob.Enqueue<GetMessages>(gm => gm.ChatHistory(command.CurrentuserId, command.ChatId, int.MaxValue));
                return Task.FromResult(0);
            }

            public Task Execute(StartFeetchingUserChatHistory command)
            {
                BackgroundJob.Enqueue<GetMessages>(gm => gm.UserChatHistory(command.CurrentuserId, command.UserId, int.MaxValue));
                return Task.FromResult(0);
            }
        }
    }
}