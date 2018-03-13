using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Auth.FWT.API.CQRS;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Events;
using Auth.FWT.Infrastructure.Telegram;
using Hangfire;
using Hangfire.Server;
using Newtonsoft.Json;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Job.Fetch
{
    public class FetchMessages
    {
        public class BaseCommand : ICommand, IPerformContext
        {
            public int CurrentUserId { get; set; }

            public Guid JobId { get; set; }

            public int MaxId { get; set; }

            public PerformContext PerformContext { get; set; }
        }

        public class FetChannalMessages : BaseCommand
        {
            public int ChannalId { get; set; }
        }

        public class FetChatMessages : BaseCommand
        {
            public int ChatId { get; set; }
        }

        public class FetchUserMessages : BaseCommand
        {
            public int UserId { get; set; }
        }

        public class Handler : ICommandHandler<FetchUserMessages>, ICommandHandler<FetChatMessages>, ICommandHandler<FetChannalMessages>
        {
            private ITelegramMessagesParser _parser;

            private Random _random;

            private ISessionStore _sessionStore;

            private ITelegramClient _telegramClient;

            private IUnitOfWork _unitOfWork;

            private IUserSessionManager _userSessionManager;

            public Handler(ITelegramClient telegramClient, IUserSessionManager userSessionManager, ISessionStore sessionStore, IUnitOfWork unitOfWork, ITelegramMessagesParser parser)
            {
                _telegramClient = telegramClient;
                _userSessionManager = userSessionManager;
                _sessionStore = sessionStore;
                _unitOfWork = unitOfWork;
                _parser = parser;
                _random = new Random();
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public Task Execute(FetChannalMessages command)
            {
                var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(command.CurrentUserId.ToString(), _sessionStore);
                var result = _telegramClient.GetChannalHistory(userSession, command.ChannalId, command.MaxId);
                var maxId = ProcessMessages(result, command.JobId);

                if (maxId > 0)
                {
                    BackgroundJob.Schedule<HangfireCommandDispatcher>(gm =>
                    gm.Dispatch(new FetChannalMessages()
                    {
                        ChannalId = command.ChannalId,
                        CurrentUserId = command.CurrentUserId,
                        JobId = command.JobId,
                        MaxId = maxId,
                    }, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                }

                return Task.CompletedTask;
            }

            public Task Execute(FetChatMessages command)
            {
                try
                {
                    var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(command.CurrentUserId.ToString(), _sessionStore);
                    var result = _telegramClient.GetChatHistory(userSession, command.ChatId, command.MaxId);
                    var maxId = ProcessMessages(result, command.JobId);

                    if (maxId > 0)
                    {
                        BackgroundJob.Schedule<HangfireCommandDispatcher>(gm => gm.Dispatch(new FetChatMessages()
                        {
                            ChatId = command.ChatId,
                            CurrentUserId = command.CurrentUserId,
                            JobId = command.JobId,
                            MaxId = maxId,
                        }, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                    }
                }
                catch
                {
                    Events.Add(new TelegramMessagesFetchingFailed()
                    {
                        JobId = command.JobId,
                        SubJobId = command.PerformContext.BackgroundJob.Id.To<long>()
                    });
                }

                return Task.CompletedTask;
            }

            public Task Execute(FetchUserMessages command)
            {
                try
                {
                    var userSession = AppUserSessionManager.Instance.UserSessionManager.Get(command.CurrentUserId.ToString(), _sessionStore);
                    var result = _telegramClient.GetUserChatHistory(userSession, command.UserId, command.MaxId);
                    var maxId = ProcessMessages(result, command.JobId);

                    if (maxId > 0)
                    {
                        BackgroundJob.Schedule<HangfireCommandDispatcher>(gm => gm.Dispatch(new FetchUserMessages()
                        {
                            UserId = command.UserId,
                            CurrentUserId = command.CurrentUserId,
                            JobId = command.JobId,
                            MaxId = maxId,
                        }, null), TimeSpan.FromSeconds(_random.Next(5, 40)));
                    }
                }
                catch
                {
                    Events.Add(new TelegramMessagesFetchingFailed()
                    {
                        JobId = command.JobId,
                        SubJobId = command.PerformContext.BackgroundJob.Id.To<long>()
                    });
                }

                return Task.CompletedTask;
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
                            parsedMessages.Add(_parser.ParseMessage(message));
                        }

                        _unitOfWork.TelegramJobDataRepository.Insert(new TelegramJobData()
                        {
                            Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parsedMessages)),
                            JobId = jobId
                        });
                        _unitOfWork.SaveChanges();

                        maxId = messagesSlice.Messages[messagesSlice.Messages.Count - 1].GetStructValuesOf<int>("Id");

                        Events.Add(new TelegramMessagesFetched()
                        {
                            JobId = jobId,
                            FetchedCount = messagesSlice.Messages.Count,
                            Total = messagesSlice.Count
                        });
                    }
                    else
                    {
                        Events.Add(new AllTelegramMessagesFetched()
                        {
                            JobId = jobId,
                        });
                    }
                }

                return maxId;
            }
        }
    }
}