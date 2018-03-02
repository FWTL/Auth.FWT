using System.Threading.Tasks;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Hangfire;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class StartFeetchingHistory
    {
        public class Command : ICommand
        {
            public int CurrentuserId { get; set; }
            public int? ChannelId { get; internal set; }
            public int? ChatId { get; set; }
            public int? UserId { get; internal set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private ITelegramClient _telegramClient;
            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
            }

            public Task Execute(Command command)
            {
                if (command.UserId.HasValue)
                {
                    BackgroundJob.Enqueue<GetMessages>(gm => gm.UserChatHistory(command.CurrentuserId, command.UserId.Value, int.MaxValue));
                }

                return Task.FromResult(0);
            }
        }
    }
}