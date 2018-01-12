using TLSharp.Core;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface IAppTelegramClient
    {
        TelegramClient Client { get; }
    }
}