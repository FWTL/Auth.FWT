using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.Entities.Identity
{
    public class UserRole : BaseEntity<int>, IRole<int>
    {
        public virtual ICollection<RoleClaim> Claims { get; set; }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
