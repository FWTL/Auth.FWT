using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace Auth.FWT.API.Controllers.Chat
{
    public class GetUserChats
    {
        public class Query : IQuery
        {
            public int Userid { get; set; }

            public Query(int userId)
            {
                Userid = userId;
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

            public async Task<List<Result>> Handle(Query query)
            {
                TLAbsDialogs absDialogs = await _telegramClient.GetUserDialogsAsync(_userSession);
                if (absDialogs is TLDialogsSlice)
                {
                    throw new Exception("TLDialogsSlice not supported");
                }

                TLDialogs dialogs = absDialogs as TLDialogs;
                var results = new List<Result>();

                var chats = dialogs.Chats.GetValuesOf("Id", "Title", "MigratedTo");
                var users = dialogs.Users.GetValuesOf("Id", "FirstName", "LastName", "Username");

                foreach (var dialog in dialogs.Dialogs)
                {
                    if (dialog.Peer is TLPeerChat)
                    {
                        var peer = dialog.Peer as TLPeerChat;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChatId);
                        if (chat["MigratedTo"] == null)
                        {
                            results.Add(new Result()
                            {
                                Id = (int)chat["Id"],
                                Title = (string)chat["Title"]
                            });
                        }
                    }

                    if (dialog.Peer is TLPeerChannel)
                    {
                        var peer = dialog.Peer as TLPeerChannel;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChannelId);
                        results.Add(new Result()
                        {
                            Id = (int)chat["Id"],
                            Title = (string)chat["Title"]
                        });
                    }

                    if (dialog.Peer is TLPeerUser)
                    {
                        var peer = dialog.Peer as TLPeerUser;
                        var user = users.FirstOrDefault(c => (int)c["Id"] == peer.UserId);

                        var name = $"{(string)user["FirstName"]} {(string)user["LastName"]}" ?? (string)user["Username"];

                        results.Add(new Result()
                        {
                            Id = (int)user["Id"],
                            Title = name
                        });
                    }
                }

                foreach (var dialog in dialogs.Dialogs)
                {
                    if (dialog.Peer is TLPeerChat)
                    {
                        var peer = dialog.Peer as TLPeerChat;
                        var chat = chats.FirstOrDefault(c => (int)c["Id"] == peer.ChatId);
                        if (chat["MigratedTo"] != null && chat["MigratedTo"] is TLInputChannel)
                        {
                            var inputChannel = chat["MigratedTo"] as TLInputChannel;
                            var channel = results.FirstOrDefault(c => c.Id == inputChannel.ChannelId);
                            channel.MigratedFrom = peer.ChatId;
                        }
                    }
                }

                return results;
            }
        }

        public class Result
        {
            public int Id { get; set; }

            public int? MigratedFrom { get; set; }

            public string Title { get; set; }
        }
    }
}