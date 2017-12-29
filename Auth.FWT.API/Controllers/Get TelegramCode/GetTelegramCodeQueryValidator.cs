using Auth.FWT.Core.Services.Telegram;
using FluentValidation;

namespace Auth.FWT.API.Controllers.Get_TelegramCode
{
    public class GetTelegramCodeQueryValidator : AbstractValidator<GetTelegramCodeQuery>
    {
        public GetTelegramCodeQueryValidator(IAppTelegramClient telegramClient)
        {
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.PhoneNumber).CustomAsync(async (phone, context, token) =>
            {
                if (await telegramClient.Client.IsPhoneRegisteredAsync(phone))
                {
                    context.AddFailure("Phone number not registred in Telegram API");
                }
            });
        }
    }
}