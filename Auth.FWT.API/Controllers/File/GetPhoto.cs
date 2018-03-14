using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using TeleSharp.TL;

namespace Auth.FWT.API.Controllers.File
{
    public class GetPhoto
    {
        public class Query : IQuery
        {
            public int LocalId { get; set; }
            public long Secret { get; set; }
            public long VolumeId { get; set; }
        }

        public class Handler : IQueryHandler<Query, byte[]>
        {
            private ITelegramClient _telegramClient;

            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public Task<byte[]> Handle(Query query)
            {
                var bytes = _telegramClient.GetFile(_userSession,
                new TLInputFileLocation()
                {
                    LocalId = query.LocalId,
                    Secret = query.Secret,
                    VolumeId = query.VolumeId,
                },
                1024);

                return Task.FromResult(bytes);
            }
        }
    }
}