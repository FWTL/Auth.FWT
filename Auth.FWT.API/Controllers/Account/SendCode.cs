using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Helpers;
using Auth.FWT.Core.Services.Telegram;
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
            private ISessionStore _sessionStore;
            private ITelegramClient _telegramClient;
            private IUnitOfWork _unitOfWork;
            private IUserSessionManager _userManager;

            public Handler(ITelegramClient telegramClient, IUnitOfWork unitOfWork, IClock clock, IUserSessionManager userManager, ISessionStore sessionStore)
            {
                _telegramClient = telegramClient;
                _unitOfWork = unitOfWork;
                _clock = clock;
                _userManager = userManager;
                _sessionStore = sessionStore;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task Execute(Command command)
            {
                var userSession = _userManager.Get(HashHelper.GetHash(command.PhoneNumber), _sessionStore);
                string hash = _telegramClient.SendCodeRequest(userSession, command.PhoneNumber);

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
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
            }
        }
    }
}