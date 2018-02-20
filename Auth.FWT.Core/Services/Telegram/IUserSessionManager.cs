using TLSharp.Core;
using TLSharp.Custom;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface IUserSessionManager
    {
        void Add(string key, UserSession userSession);

        UserSession Get(string key, ISessionStore store = null);

        void Replace(string oldKey, string newKey);
    }
}