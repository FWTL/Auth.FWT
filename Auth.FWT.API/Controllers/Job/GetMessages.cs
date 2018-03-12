using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Events;
using Auth.FWT.Infrastructure.Telegram;
using Hangfire;
using Hangfire.Server;
using Newtonsoft.Json;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Job
{
    public class GetMessages
    {
        private Random _random;
        private ISessionStore _sessionStore;
        private ITelegramClient _telegramClient;
        private IUserSessionManager _userSessionManager;
        private IServiceBus _serviceBus;
        private IUnitOfWork _unitOfWork;
        
        public GetMessages(ITelegramClient telegramClient, IUserSessionManager userSessionManager, ISessionStore sessionStore, IServiceBus serviceBus, IUnitOfWork unitOfWork)
        {
            _telegramClient = telegramClient;
            _userSessionManager = userSessionManager;
            _sessionStore = sessionStore;
            _serviceBus = serviceBus;
            _unitOfWork = unitOfWork;
            _random = new Random();
        }

        [AutomaticRetry(Attempts = 0)]
        public void UserChatHistory(int userId, int chatId, int maxId, Guid jobId, PerformContext hangfireContext)
        {
            try
            {
                var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
                var result = _telegramClient.GetUserChatHistory(userSession, chatId, maxId);
                maxId = ProcessMessages(result, jobId);

                if (maxId > 0)
                {
                    BackgroundJob.Schedule<GetMessages>(gm => gm.UserChatHistory(userId, chatId, maxId, jobId, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                }
            }
            catch
            {
                new TelegramMessagesFetchingFailed()
                {
                    JobId = jobId,
                    SubJobId = hangfireContext.BackgroundJob.Id.To<long>()
                }.Send(_serviceBus);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void ChannalHistory(int userId, int channalId, int maxId, Guid jobId, PerformContext hangfireContext)
        {
            try
            {
                var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
                var result = _telegramClient.GetChannalHistory(userSession, channalId, maxId);
                maxId = ProcessMessages(result, jobId);

                if (maxId > 0)
                {
                    BackgroundJob.Schedule<GetMessages>(gm => gm.ChannalHistory(userId, channalId, maxId, jobId, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                }
            }
            catch
            {
                new TelegramMessagesFetchingFailed()
                {
                    JobId = jobId,
                    SubJobId = hangfireContext.BackgroundJob.Id.To<long>()
                }.Send(_serviceBus);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void ChatHistory(int userId, int chatId, int maxId, Guid jobId, PerformContext hangfireContext)
        {
            try
            {
                var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(userId.ToString(), _sessionStore);
                var result = _telegramClient.GetChatHistory(userSession, chatId, maxId);
                maxId = ProcessMessages(result, jobId);

                if (maxId > 0)
                {
                    BackgroundJob.Schedule<GetMessages>(gm => gm.ChatHistory(userId, chatId, maxId, jobId, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                }
            }
            catch
            {
                new TelegramMessagesFetchingFailed()
                {
                    JobId = jobId,
                    SubJobId = hangfireContext.BackgroundJob.Id.To<long>()
                }.Send(_serviceBus);
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
                    var parsedMessages = new List<TelegramMessage>();
                    foreach (var message in messagesSlice.Messages)
                    {
                        parsedMessages.Add(ProcessMessages(message));
                    }

                    _unitOfWork.TelegramJobDataRepository.Insert(new TelegramJobData()
                    {
                        Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parsedMessages)),
                        JobId = jobId
                    });
                    _unitOfWork.SaveChanges();

                    maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");

                    new TelegramMessagesFetched()
                    {
                        JobId = jobId,
                        FetchedCount = messagesSlice.Messages.Count,
                        Total = messagesSlice.Count
                    }.Send(_serviceBus);
                }
                else
                {
                    new AllTelegramMessagesFetched()
                    {
                        JobId = jobId,
                    }.Send(_serviceBus);
                }
            }

            return maxId;
        }

        private TelegramMessage ProcessMessages(TLAbsMessage message)
        {
            if (message is TLMessage)
            {
                return new TelegramMessage(message as TLMessage);
            }
            else if (message is TLMessageService)
            {
                return new TelegramMessage(message as TLMessageService);
            }
            else if (message is TLMessageEmpty)
            {
            }

            return null;
        }
    }
}