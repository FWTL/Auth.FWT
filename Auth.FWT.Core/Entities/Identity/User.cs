using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.Entities.Identity
{
    public class User : BaseEntity<int>, IUser<int>
    {
        public User()
        {
            Claims = new HashSet<UserClaim>();
            Roles = new HashSet<UserRole>();
        }

        public User(string phoneNumberHashed, DateTime stamp)
        {
            PhoneNumberHashed = phoneNumberHashed;
            SecurityStamp = stamp.Ticks.ToString();
            UserName = "User" + stamp;
        }

        public virtual ICollection<UserClaim> Claims { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public string SecurityStamp { get; set; }

        public virtual TelegramSession TelegramSession { get; set; }

        public string UserName { get; set; }

        public string PhoneNumberHashed { get; set; }
    }
}