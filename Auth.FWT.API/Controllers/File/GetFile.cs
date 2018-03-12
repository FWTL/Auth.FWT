using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleSharp.TL;

namespace Auth.FWT.API.Controllers.File
{
    public class GetFile
    {
        public class Query : IQuery
        {
            public long AccessHash { get; set; }

            public long Id { get; set; }

            public int Size { get; set; }

            public int Version { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.AccessHash).NotEmpty();
                RuleFor(x => x.Version).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Size).GreaterThan(0);
            }
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
                new TLInputDocumentFileLocation()
                {
                    AccessHash = query.AccessHash,
                    Id = query.Id,
                    Version = query.Version
                }, query.Size);

                return Task.FromResult(bytes);
            }
        }
    }
}