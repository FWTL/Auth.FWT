using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TeleSharp.TL.Upload;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface ITelegramClient
    {
        TLAbsMessages GetChannalHistory(UserSession userSession, int channalId, int maxId, int limit = 100);

        TLAbsMessages GetChatHistory(UserSession userSession, int chatId, int maxId, int limit = 100);

        byte[] GetFile(UserSession userSession, TLAbsInputFileLocation location, int size);

        TLAbsMessages GetUserChatHistory(UserSession session, int userChatId, int maxId, int limit = 100);

        TLAbsDialogs GetUserDialogs(UserSession session);

        bool IsPhoneRegistered(UserSession userSession, string phoneNumber);

        UserSession MakeAuth(UserSession userSession, string phoneNumber, string phoneCodeHash, string code);

        string SendCodeRequest(UserSession userSession, string phoneNumber);
    }
}
