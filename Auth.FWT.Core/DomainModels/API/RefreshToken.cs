using System;

namespace Auth.FWT.Core.DomainModels.API
{
    public class RefreshToken : BaseEntity<string>
    {
        public virtual ClientAPI ClientAPI { get; set; }

        public string ClientAPIId { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public DateTime IssuedUtc { get; set; }

        public string ProtectedTicket { get; set; }

        public string Subject { get; set; }
    }
}
