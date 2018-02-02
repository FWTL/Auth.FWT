using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.CQRS;
using FluentValidation;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Account
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

        public class Validator : AbstractValidator<Command>
        {
            public Validator(TelegramClient telegramClient)
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                //RuleFor(x => x.PhoneNumber).CustomAsync(async (phone, context, token) =>
                //{
                //    if (await telegramClient.IsPhoneRegisteredAsync(phone))
                //    {
                //        context.AddFailure("Phone number not registred in Telegram API");
                //    }
                //});
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private TelegramClient _telegramClient;

            public Handler(TelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public async Task Execute(Command command)
            {
                throw new System.Exception("ASdasd");
                await _telegramClient.ConnectAsync();
                await _telegramClient.IsPhoneRegisteredAsync(command.PhoneNumber);
            }
        }
    }
}