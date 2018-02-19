using System.Threading.Tasks;
using TeleSharp.TL;

namespace TLSharp.Custom
{
    public interface ITelegramClient
    {
        Task<string> SendCodeRequestAsync(UserSession userSession, string phoneNumber);
        Task<UserSession> MakeAuthAsync(UserSession userSession, string phoneNumber, string phoneCodeHash, string code);
        Task<bool> IsPhoneRegisteredAsync(UserSession userSession, string phoneNumber);
    }
}