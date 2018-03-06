using System;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Events;
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
        private IServiceBus _serviceBus;

        public GetMessages(ITelegramClient telegramClient, IUserSessionManager userSessionManager, ISessionStore sessionStore, IServiceBus serviceBus)
        {
            _telegramClient = telegramClient;
            _userSessionManager = userSessionManager;
            _sessionStore = sessionStore;
            _serviceBus = serviceBus;
            _random = new Random();
        }

        [AutomaticRetry(Attempts = 0)]
        public void UserChatHistory(int userId, int chatId, int maxId, Guid jobId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetUserChatHistory(userSession, chatId, maxId);
            maxId = ProcessMessages(result, jobId);

            if (maxId > 0)
            {
                BackgroundJob.Schedule<GetMessages>(gm => gm.ChannalHistory(userId, chatId, maxId, jobId), TimeSpan.FromSeconds(_random.Next(5, 40)));
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void ChannalHistory(int userId, int channalId, int maxId, Guid jobId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetChannalHistory(userSession, channalId, maxId);
            maxId = ProcessMessages(result, jobId);

            if (maxId > 0)
            {
                BackgroundJob.Schedule<GetMessages>(gm => gm.ChannalHistory(userId, channalId, maxId, jobId), TimeSpan.FromSeconds(_random.Next(5, 40)));
            }

        }

        [AutomaticRetry(Attempts = 0)]
        public void ChatHistory(int userId, int chatId, int maxId, Guid jobId)
        {
            var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
            var result = _telegramClient.GetChatHistory(userSession, chatId, maxId);
            maxId = ProcessMessages(result, jobId);

            if (maxId > 0)
            {
                BackgroundJob.Schedule<GetMessages>(gm => gm.ChannalHistory(userId, chatId, maxId, jobId), TimeSpan.FromSeconds(_random.Next(5, 40)));
            }
        }

        private int ProcessMessages(TLAbsMessages result, Guid jobId)
        {
            int maxId = -1;

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

                    _serviceBus.SendToQueue("processing", new TelegramMessagesFetched()
                    {
                        JobId = jobId,
                        FetchedCount = messagesSlice.Messages.Count,
                        Total = messagesSlice.Count
                    });
                }
                else
                {
                    _serviceBus.SendToQueue("processing", new AllTelegramMessagesFetched()
                    {
                        JobId = jobId,
                    });
                }
            }

            return maxId;
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