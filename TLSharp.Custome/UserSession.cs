using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace TLSharp.Custome
{
    public class UserSession
    {
        private int userid { get; set; }

        public Session Session { get; private set; }
        public TcpTransport TcpTransport { get; private set; }

        public MtProtoSender Sender { get; private set; }

        public UserSession(int userId, ISessionStore store)
        {
            userid = userId;
            Session = Session.TryLoadOrCreateNew(store, userId.ToString());
            TcpTransport = new TcpTransport(Session.ServerAddress, Session.Port);
            Sender = new MtProtoSender(TcpTransport, Session);
        }
    }
}