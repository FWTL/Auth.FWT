using System.Collections.Generic;
using static Auth.FWT.Core.Enums.DomainEnums;

namespace Auth.FWT.Core.Entities.API
{
    public class ClientAPI : BaseEntity<string>
    {
        public string AllowedOrigin { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public virtual IEnumerable<RefreshToken> RefreshTokens { get; set; }

        public string Secret { get; set; }
    }
}
