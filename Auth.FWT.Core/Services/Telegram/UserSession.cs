using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace Auth.FWT.Core.Services.Telegram
{
    public class UserSession
    {
        public Session Session { get; private set; }

        public TcpTransport TcpTransport { get; set; }

        public MtProtoSender Sender { get; set; }

        public UserSession(Session session, TcpTransport tcp)
        {
            Session = session;
            TcpTransport = tcp;
        }
    }
}