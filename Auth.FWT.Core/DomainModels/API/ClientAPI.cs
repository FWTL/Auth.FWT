using System.Collections.Generic;

namespace Auth.FWT.Core.DomainModels.API
{
    public class ClientAPI : BaseEntity<string>
    {
        public bool Active { get; set; }

        public string AllowedOrigin { get; set; }

        public Auth.FWT.Core.Enums.Enum.ApplicationType ApplicationType { get; set; }

        public string Name { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public virtual IEnumerable<RefreshToken> RefreshTokens { get; set; }

        public string Secret { get; set; }
    }
}
