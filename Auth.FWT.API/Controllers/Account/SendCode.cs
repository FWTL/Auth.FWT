using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Helpers;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Telegram;
using FluentValidation;
using NodaTime;
using TLSharp.Core;
using TLSharp.Core.Network;
using TLSharp.Custom;

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
            private ITelegramClient _telegramClient;
            private IUnitOfWork _unitOfWork;
            private IUserSessionManager _userManager;

            public Handler(ITelegramClient telegramClient, IUnitOfWork unitOfWork, IClock clock, IUserSessionManager userManager)
            {
                _telegramClient = telegramClient;
                _unitOfWork = unitOfWork;
                _clock = clock;
                _userManager = userManager;
            }

            public async Task Execute(Command command)
            {
                var userSession = new UserSession(new SQLSessionStore(_unitOfWork, _clock));
                string hash = await _telegramClient.SendCodeRequestAsync(userSession, command.PhoneNumber);
                _userManager.Add(HashHelper.GetHash(command.PhoneNumber), userSession);

                TelegramCode telegramCode = await _unitOfWork.TelegramCodeRepository.GetSingleAsync(HashHelper.GetHash(command.PhoneNumber));
                if (telegramCode.IsNull())
                {
                    telegramCode = new TelegramCode(command.PhoneNumber, hash, _clock.UtcNow());
                    _unitOfWork.TelegramCodeRepository.Insert(telegramCode);
                }
                else
                {
                    telegramCode.CodeHash = hash;
                    telegramCode.IssuedUTC = _clock.UtcNow();
                    _unitOfWork.TelegramCodeRepository.Update(telegramCode);
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(ITelegramClient telegramClient, IUnitOfWork unitOfWork, IClock clock)
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                RuleFor(x => x.PhoneNumber).CustomAsync(async (phone, context, token) =>
                {
                    try
                    {
                        var userSession = new UserSession(new FakeSessionStore());
                        if (!await telegramClient.IsPhoneRegisteredAsync(userSession, phone))
                        {
                            context.AddFailure("Phone number not registred in Telegram API");
                        }
                    }
                    catch (FloodException ex)
                    {
                        context.AddFailure(ex.Message);
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
    }
}