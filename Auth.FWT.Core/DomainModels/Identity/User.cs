using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.DomainModels.Identity
{
    public class User : BaseEntity<int>, IUser<int>
    {
        public User()
        {
            Claims = new HashSet<UserClaim>();
            Logins = new HashSet<UserLogin>();
            Roles = new HashSet<UserRole>();
        }

        public int AccessFailedCount { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UserName { get; set; }
    }
}
