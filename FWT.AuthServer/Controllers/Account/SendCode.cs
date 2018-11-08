using FluentValidation;
using FWT.Core.CQRS;
using FWT.Core.Helpers;
using FWT.Core.Services.Telegram;
using FWT.Infrastructure.Validation;
using NodaTime;
using OpenTl.ClientApi;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FWT.AuthServer.Controllers.Account
{
    public class SendCode
    {
        public class Command : ICommand
        {
            public Command(string phoneNumber)
            {
                PhoneNumber = $"+{Regex.Match(phoneNumber, @"\d+").Value}";
            }

            public string PhoneNumber { get; private set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IClock _clock;
            private readonly ITelegramService _telegramService;

            public Handler(IClock clock, ITelegramService telegramService)
            {
                _clock = clock;
                _telegramService = telegramService;
            }

            public async Task ExecuteAsync(Command command)
            {
                string hashedPhoneId = HashHelper.GetHash(command.PhoneNumber);
                IClientApi client = await _telegramService.Build(hashedPhoneId);
                await client.AuthService.SendCodeAsync(command.PhoneNumber);
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
            }
        }
    }
}