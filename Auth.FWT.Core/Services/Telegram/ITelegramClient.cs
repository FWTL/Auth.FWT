using TeleSharp.TL.Messages;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface ITelegramClient
    {
        string SendCodeRequest(UserSession userSession, string phoneNumber);

        UserSession MakeAuth(UserSession userSession, string phoneNumber, string phoneCodeHash, string code);

        bool IsPhoneRegistered(UserSession userSession, string phoneNumber);

        TLAbsDialogs GetUserDialogs(UserSession session);

        TLAbsMessages GetUserChatHistory(UserSession session, int userChatId, int maxId, int limit = 100);
    }
}