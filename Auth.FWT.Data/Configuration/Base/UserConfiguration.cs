using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.Helpers.Const;
using Auth.FWT.Domain.Entities.Identity;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(x => x.SecurityStamp).HasColumnType(ParametersName.Varchar).HasMaxLength(36);
        }
    }
}
