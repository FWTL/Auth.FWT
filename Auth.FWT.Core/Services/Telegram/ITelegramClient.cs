using System.Threading.Tasks;
using TeleSharp.TL.Messages;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface ITelegramClient
    {
        Task<string> SendCodeRequestAsync(UserSession userSession, string phoneNumber);

        Task<UserSession> MakeAuthAsync(UserSession userSession, string phoneNumber, string phoneCodeHash, string code);

        Task<bool> IsPhoneRegisteredAsync(UserSession userSession, string phoneNumber);

        Task<TLAbsDialogs> GetUserDialogsAsync(UserSession session);

        Task<TLAbsMessages> GetUserChatHistory(UserSession session, int userChatId, int maxId, int limit = 100);
    }
}