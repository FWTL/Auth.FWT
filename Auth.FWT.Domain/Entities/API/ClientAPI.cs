using System.Collections.Generic;
using static Auth.FWT.Domain.Enums.Enum;

namespace Auth.FWT.Domain.Entities.API
{
    public class ClientAPI : BaseEntity<string>
    {
        public bool Active { get; set; }

        public string AllowedOrigin { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public string Name { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public virtual IEnumerable<RefreshToken> RefreshTokens { get; set; }

        public string Secret { get; set; }
    }
}
