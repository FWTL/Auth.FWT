using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Helpers;
using Auth.FWT.CQRS;
using FluentValidation;
using NodaTime;
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

        public class Handler : ICommandHandler<Command>
        {
            private IClock _clock;
            private TelegramClient _telegramClient;
            private IUnitOfWork _unitOfWork;

            public Handler(TelegramClient telegramClient, IUnitOfWork unitOfWork, IClock clock)
            {
                _telegramClient = telegramClient;
                _unitOfWork = unitOfWork;
                _clock = clock;
            }

            public async Task Execute(Command command)
            {
                string hash = await _telegramClient.SendCodeRequestAsync(command.PhoneNumber);

                TelegramCode telegramCode = await _unitOfWork.TelegramCodeRepository.GetSingleAsync(HashHelper.GetHash(command.PhoneNumber));
                if (telegramCode.IsNull())
                {
                    telegramCode = new TelegramCode(command.PhoneNumber, hash, _clock.UtcNow());
                    _unitOfWork.TelegramCodeRepository.Insert(telegramCode);
                }
                else
                {
                    telegramCode.CodeHash = telegramCode.CodeHash;
                    telegramCode.IssuedUTC = _clock.UtcNow();
                    _unitOfWork.TelegramCodeRepository.Update(telegramCode);
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(TelegramClient telegramClient, IUnitOfWork unitOfWork, IClock clock)
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                RuleFor(x => x.PhoneNumber).CustomAsync(async (phone, context, token) =>
                {
                    try
                    {
                        if (!await telegramClient.IsPhoneRegisteredAsync(phone))
                        {
                            context.AddFailure("Phone number not registred in Telegram API");
                        }
                    }
                    catch (Exception ex)
                    {
                        context.AddFailure(ex.Message);
                    }
                });
            }
        }
    }
}