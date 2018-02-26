using System.Collections.Generic;
using Auth.FWT.Core.Services.Telegram;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class UserSessionManager : IUserSessionManager
    {
        public Dictionary<string, UserSession> Sessions { get; set; } = new Dictionary<string, UserSession>();

        public Dictionary<string, TcpTransport> Connections { get; set; } = new Dictionary<string, TcpTransport>();

        public UserSession Get(string key, ISessionStore store)
        {
            if (Sessions.ContainsKey(key))
            {
                return Sessions[key];
            }

            var session = Session.TryLoadOrCreateNew(store, key);
            TcpTransport transport = GetConnection(session.ServerAddress, session.Port);

            Sessions[key] = new UserSession(session, transport);
            return Sessions[key];
        }

        public TcpTransport GetConnection(string address, int port)
        {
            if (Connections.ContainsKey($"{address}:{port}"))
            {
                var transport = Connections[$"{address}:{port}"];
                if (transport.IsConnected)
                {
                    return transport;
                }

                return Connections[$"{address}:{port}"] = new TcpTransport(address, port);
            }
            else
            {
                var transport = new TcpTransport(address, port);
                Connections.Add($"{address}:{port}", transport);
                return transport;
            }
        }

        public void Replace(string oldKey, string newKey)
        {
            Sessions.Add(newKey, Sessions[oldKey]);
            Sessions.Remove(oldKey);
        }
    }
}