using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Infrastructure.Telegram;
using Hangfire;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Statistics
{
    public class GetMessages
    {
        private Random _random;
        private ISessionStore _sessionStore;
        private ITelegramClient _telegramClient;
        private IUserSessionManager _userSessionManager;

        public GetMessages(ITelegramClient telegramClient, IUserSessionManager userSessionManager, ISessionStore sessionStore)
        {
            _telegramClient = telegramClient;
            _userSessionManager = userSessionManager;
            _sessionStore = sessionStore;
            _random = new Random();
        }

        public async Task UserChatHistory(int userId, int chatId, int maxId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = await _telegramClient.GetUserChatHistory(userSession, chatId, maxId);
            if (result is TLMessagesSlice)
            {
                var messagesSlice = result as TLMessagesSlice;
                if (messagesSlice.Messages.Count > 0)
                {
                    maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");

                    BackgroundJob.Schedule<GetMessages>(gm => gm.UserChatHistory(userId, chatId, maxId), TimeSpan.FromMinutes(_random.Next(5, 20)));
                }
            }
        }
    }
}