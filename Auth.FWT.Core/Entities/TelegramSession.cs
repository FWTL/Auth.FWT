using System;
using Auth.FWT.Core.Entities.Identity;
using TLSharp.Core;

namespace Auth.FWT.Core.Entities
{
    public class TelegramSession : BaseEntity<int>
    {
        public TelegramSession()
        {
        }

        public TelegramSession(Session session, User currentUser)
        {
            Id = currentUser.Id;
            Session = session.ToBytes();
            ExpireDateUtc = DateTimeOffset.FromUnixTimeSeconds(session.SessionExpires).UtcDateTime;
        }

        public DateTime ExpireDateUtc { get; set; }

        public byte[] Session { get; set; }

        public virtual User User { get; set; }
    }
}