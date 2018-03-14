using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Handlers;
using StackExchange.Redis;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace Auth.FWT.API.Controllers.Chat
{
    public class GetUserChats
    {
        public class Query : IQuery
        {
            public int UserId { get; set; }

            public bool DoRefresh { get; private set; }

            public Query(int userId, bool doRefresh)
            {
                UserId = userId;
                DoRefresh = doRefresh;
            }
        }

        public class Cache : RedisJsonHandler<Query, List<Result>>
        {
            public Cache(IDatabase redis) : base(redis)
            {
                KeyFn = query => { return "GetUserChats" + query.UserId; };
            }

            public override async Task<List<Result>> Read(Query query)
            {
                if (query.DoRefresh)
                {
                    return null;
                }

                return await base.Read(query);
            }
        }

        public class Handler : IQueryHandler<Query, List<Result>>
        {
            private ITelegramClient _telegramClient;
            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public Task<List<Result>> Handle(Query query)
            {
                TLAbsDialogs absDialogs = _telegramClient.GetUserDialogs(_userSession);
                if (absDialogs is TLDialogsSlice)
                {
                    throw new Exception("TLDialogsSlice not supported");
                }

                TLDialogs dialogs = absDialogs as TLDialogs;
                var results = new List<Result>();

                var chats = dialogs.Chats.GetListOfValuesOf("Id", "Title", "MigratedTo", "Photo");
                var users = dialogs.Users.GetListOfValuesOf("Id", "FirstName", "LastName", "Username", "Photo");

                foreach (var dialog in dialogs.Dialogs)
                {
                    if (dialog.Peer is TLPeerChat)
                    {
                        var peer = dialog.Peer as TLPeerChat;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChatId);
                        var photo = chat["Photo"]?.GetRefValuesOf<TLFileLocation>("PhotoBig");

                        if (chat["MigratedTo"] == null)
                        {
                            results.Add(new Result()
                            {
                                Title = (string)chat["Title"],
                                ChatId = peer.ChatId,
                                PhotoLocation = photo.IsNotNull() ? new PhotoLocation(photo) : null
                            });
                        }
                    }

                    if (dialog.Peer is TLPeerChannel)
                    {
                        var peer = dialog.Peer as TLPeerChannel;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChannelId);
                        var photo = chat["Photo"]?.GetRefValuesOf<TLFileLocation>("PhotoBig");

                        results.Add(new Result()
                        {
                            Title = (string)chat["Title"],
                            ChannelId = peer.ChannelId,
                            PhotoLocation = photo.IsNotNull() ? new PhotoLocation(photo) : null
                        });
                    }

                    if (dialog.Peer is TLPeerUser)
                    {
                        var peer = dialog.Peer as TLPeerUser;
                        var user = users.FirstOrDefault(c => (int)c["Id"] == peer.UserId);
                        var photo = user["Photo"]?.GetRefValuesOf<TLFileLocation>("PhotoBig");

                        var name = $"{(string)user["FirstName"]} {(string)user["LastName"]}" ?? (string)user["Username"];

                        results.Add(new Result()
                        {
                            Title = name,
                            UserId = peer.UserId,
                            PhotoLocation = photo.IsNotNull() ? new PhotoLocation(photo) : null
                        });
                    }
                }

                foreach (var dialog in dialogs.Dialogs)
                {
                    if (dialog.Peer is TLPeerChat)
                    {
                        var peer = dialog.Peer as TLPeerChat;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChatId);
                        var photo = chat.GetRefValuesOf<TLFileLocation>("PhotoBig");

                        if (chat["MigratedTo"] != null && chat["MigratedTo"] is TLInputChannel)
                        {
                            var inputChannel = chat["MigratedTo"] as TLInputChannel;
                            var channel = results.FirstOrDefault(c => c.ChannelId == inputChannel.ChannelId);
                            channel.ChatId = peer.ChatId;
                        }
                    }
                }

                return Task.FromResult(results);
            }
        }

        public class Result
        {
            public int? ChatId { get; set; }

            public int? ChannelId { get; set; }

            public int? UserId { get; set; }

            public string Title { get; set; }

            public PhotoLocation PhotoLocation { get; set; }
        }

        public class PhotoLocation
        {
            public PhotoLocation()
            {
            }

            public PhotoLocation(TLFileLocation photo)
            {
                LocalId = photo.LocalId;
                Secret = photo.Secret;
                VolumeId = photo.VolumeId;
            }

            public int LocalId { get; set; }

            public long Secret { get; set; }

            public long VolumeId { get; set; }
        }
    }
}