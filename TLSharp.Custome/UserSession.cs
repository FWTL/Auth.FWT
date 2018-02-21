using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace TLSharp.Custom
{
    public class UserSession
    {
        public Session Session { get; private set; }
        public TcpTransport TcpTransport { get; set; }

        public MtProtoSender Sender { get; set; }

        public UserSession(ISessionStore store)
        {
            Session = Session.TryLoadOrCreateNew(store, string.Empty);
            TcpTransport = new TcpTransport(Session.ServerAddress, Session.Port);
        }

        public UserSession(int userId, ISessionStore store)
        {
            Session = Session.TryLoadOrCreateNew(store, userId.ToString());
            TcpTransport = new TcpTransport(Session.ServerAddress, Session.Port);
        }
    }
}