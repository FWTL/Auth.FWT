using TeleSharp.TL;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface ITelegramMessagesParser
    {
        TelegramMessage ParseMessage(TLAbsMessage message);
    }
}