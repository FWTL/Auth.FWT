using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.Entities.Identity;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserRoleConfiguration : EntityTypeConfiguration<UserRole>
    {
        public UserRoleConfiguration()
        {
        }
    }
}
