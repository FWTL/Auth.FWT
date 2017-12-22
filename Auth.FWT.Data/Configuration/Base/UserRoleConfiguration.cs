using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.DomainModels.Identity;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserRoleConfiguration : EntityTypeConfiguration<UserRole>
    {
        public UserRoleConfiguration()
        {
        }
    }
}
