using System.Collections.Generic;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using TLSharp.Core;
using TLSharp.Custom;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class UserSessionManager : IUserSessionManager
    {
        public Dictionary<string, UserSession> Sessions = new Dictionary<string, UserSession>();

        public void Add(string key, UserSession userSession)
        {
            if (Sessions.ContainsKey(key))
            {
                Sessions[key] = userSession;
                return;
            }

            Sessions.Add(key, userSession);
        }

        public UserSession Get(string key, ISessionStore store = null)
        {
            if (Sessions.ContainsKey(key))
            {
                return Sessions[key];
            }

            int? id = key.ToN<int>();
            if (!id.HasValue)
            {
                return null;
            }

            Sessions[key] = new UserSession(id.Value, store);
            return Sessions[key];
        }

        public void Replace(string oldKey, string newKey)
        {
            Sessions.Add(newKey, Sessions[oldKey]);
            Sessions.Remove(oldKey);
        }
    }
}