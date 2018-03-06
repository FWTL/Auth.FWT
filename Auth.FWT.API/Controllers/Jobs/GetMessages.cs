using System;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Infrastructure.Telegram;
using Hangfire;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Jobs
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

        [AutomaticRetry(Attempts = 0)]
        public void UserChatHistory(int userId, int chatId, int maxId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetUserChatHistory(userSession, chatId, maxId);
            if (result is TLMessagesSlice)
            {
                var messagesSlice = result as TLMessagesSlice;
                if (messagesSlice.Messages.Count > 0)
                {
                    foreach (var message in messagesSlice.Messages)
                    {
                        ProcessMessages(message);
                    }

                    maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");
                    BackgroundJob.Schedule<GetMessages>(gm => gm.UserChatHistory(userId, chatId, maxId), TimeSpan.FromSeconds(_random.Next(5, 20)));
                }
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void ChannalHistory(int userId, int channalId, int maxId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetChannalHistory(userSession, channalId, maxId);
            if (result is TLMessagesSlice)
            {
                var messagesSlice = result as TLMessagesSlice;
                if (messagesSlice.Messages.Count > 0)
                {
                    foreach (var message in messagesSlice.Messages)
                    {
                        ProcessMessages(message);
                    }

                    maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");
                    BackgroundJob.Schedule<GetMessages>(gm => gm.ChannalHistory(userId, channalId, maxId), TimeSpan.FromSeconds(_random.Next(5, 20)));
                }
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void ChatHistory(int userId, int chatId, int maxId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetChatHistory(userSession, chatId, maxId);
            if (result is TLMessagesSlice)
            {
                var messagesSlice = result as TLMessagesSlice;
                if (messagesSlice.Messages.Count > 0)
                {
                    foreach (var message in messagesSlice.Messages)
                    {
                        ProcessMessages(message);
                    }

                    maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");
                    BackgroundJob.Schedule<GetMessages>(gm => gm.ChatHistory(userId, chatId, maxId), TimeSpan.FromSeconds(_random.Next(5, 20)));
                }
            }
        }

        private void ProcessMessages(TLAbsMessage message)
        {
            if (message is TLMessage)
            {
                new TelegramMessage(message as TLMessage);
            }
            else if (message is TLMessageService)
            {
                new TelegramMessage(message as TLMessageService);
            }
            else if (message is TLMessageEmpty)
            {
            }
        }
    }
}