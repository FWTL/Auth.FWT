using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Helpers;
using Auth.FWT.CQRS;
using Auth.FWT.Domain.Entities;
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
                RuleFor(x => x.PhoneNumber).CustomAsync(async (phone, context, token) =>
                {
                    await telegramClient.ConnectAsync();
                    try
                    {
                        if (!await telegramClient.IsPhoneRegisteredAsync(phone))
                        {
                            context.AddFailure("Phone number not registred in Telegram API");
                        }
                    }
                    catch (Exception ex)
                    {
                        switch (ex.Message)
                        {
                            case ("PHONE_NUMBER_BANNED"):
                            case ("PHONE_NUMBER_INVALID"):
                                {
                                    context.AddFailure(ex.Message);
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("Unexpected error", ex);
                                }
                        }
                    }
                });
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private TelegramClient _telegramClient;
            private IUnitOfWork _unitOfWork;

            public Handler(TelegramClient telegramClient, IUnitOfWork unitOfWork)
            {
                _telegramClient = telegramClient;
                _unitOfWork = unitOfWork;
            }

            public async Task Execute(Command command)
            {
                await _telegramClient.ConnectAsync();
                string hash = await _telegramClient.SendCodeRequestAsync(command.PhoneNumber);

                TelegramCode telegramCode = await _unitOfWork.TelegramCodeRepository.GetSingleAsync(HashHelper.GetHash(command.PhoneNumber));
                if (telegramCode.IsNull())
                {

                }
            }
        }
    }
}