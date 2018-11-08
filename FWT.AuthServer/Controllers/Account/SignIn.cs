using FluentValidation;
using FWT.Core.CQRS;
using FWT.Core.Helpers;
using FWT.Core.Services.Telegram;
using FWT.Infrastructure.Validation;
using NodaTime;
using OpenTl.ClientApi;
using OpenTl.Schema;
using OpenTl.Schema.Auth;
using OpenTl.Schema.Help;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FWT.AuthServer.Controllers.Account
{
    public class SignIn
    {
        public class Query : IQuery
        {
            public Query(string phoneNumber, string sentCode, string code)
            {
                PhoneNumber = $"+{Regex.Match(phoneNumber, @"\d+").Value}";
                Code = code;
                SentCode = sentCode;
            }

            public string PhoneNumber { get; }
            public string Code { get; }
            public string SentCode { get; }
        }

        public class Handler : IQueryHandler<Query, string>
        {
            private readonly IClock _clock;
            private readonly ITelegramService _telegramService;

            public Handler(IClock clock, ITelegramService telegramService)
            {
                _clock = clock;
                _telegramService = telegramService;
            }

            public async Task<string> HandleAsync(Query query)
            {
                string hashedPhoneId = HashHelper.GetHash(query.PhoneNumber);
                IClientApi client = await _telegramService.Build(hashedPhoneId);

                var sentCode = new FakeSendCode()
                {
                    PhoneCodeHash = query.SentCode
                };

                TUser result = await client.AuthService.SignInAsync(query.PhoneNumber, sentCode, query.Code);
                return string.Empty;
            }
        }

        public class Validator : AppAbstractValidation<Query>
        {
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                RuleFor(x => x.Code).NotEmpty();
                RuleFor(x => x.SentCode).NotEmpty();
            }
        }

        public class FakeSendCode : ISentCode
        {
            public BitArray Flags { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public bool PhoneRegistered { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public ISentCodeType Type { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public byte[] PhoneCodeHashAsBinary { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public string PhoneCodeHash { get;set; }
            public ICodeType NextType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public int Timeout { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public ITermsOfService TermsOfService { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        }
    }
}