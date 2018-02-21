using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.Entities.Identity;
using Auth.FWT.Core.Helpers.Const;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(x => x.SecurityStamp).HasColumnType(ParametersName.Varchar).HasMaxLength(36);
            Property(x => x.PhoneNumberHashed).HasColumnType(ParametersName.Varchar).IsMaxLength();
            HasOptional(x => x.TelegramSession).WithRequired(x => x.User);
        }
    }
}