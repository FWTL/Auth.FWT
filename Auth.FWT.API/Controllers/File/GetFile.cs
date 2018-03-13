using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using FluentValidation;
using TeleSharp.TL;
using TeleSharp.TL.Upload;

namespace Auth.FWT.API.Controllers.File
{
    public class GetFile
    {
        public class Handler : IQueryHandler<Query, Result>
        {
            private ITelegramClient _telegramClient;

            private UserSession _userSession;

            public Handler(ITelegramClient telegramClient, UserSession userSession)
            {
                _telegramClient = telegramClient;
                _userSession = userSession;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public Task<Result> Handle(Query query)
            {
                TLFile file = _telegramClient.GetFile(
                _userSession,
                new TLInputDocumentFileLocation()
                {
                    AccessHash = query.AccessHash,
                    Id = query.Id,
                    Version = query.Version
                },
                query.Size);

                return Task.FromResult(new Result() { File = file });
            }
        }

        public class Query : IQuery
        {
            public long AccessHash { get; set; }

            public long Id { get; set; }

            public int Size { get; set; }

            public int Version { get; set; }
        }

        public class Result
        {
            public TLFile File { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.AccessHash).NotEmpty();
                RuleFor(x => x.Version).GreaterThanOrEqualTo(0);
            }
        }
    }
}
