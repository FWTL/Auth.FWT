using TLSharp.Core;
using TLSharp.Core.Network;

namespace Auth.FWT.Core.Services.Telegram
{
    public interface IUserSessionManager
    {
        UserSession Get(string key, ISessionStore store);

        void Replace(string oldKey, string newKey);

        TcpTransport GetConnection(string address, int port);
    }
}