using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.DomainModels.Identity;
using Auth.FWT.Core.Helpers.Const;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(x => x.PasswordHash).HasColumnType(ParametersName.Varchar).HasMaxLength(48);
            Property(x => x.SecurityStamp).HasColumnType(ParametersName.Varchar).HasMaxLength(36);
        }
    }
}
