using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.DomainModels.Identity;

namespace Auth.FWT.Data.Base.Configuration
{
    public class UserLoginConfiguration : EntityTypeConfiguration<UserLogin>
    {
        public UserLoginConfiguration()
        {
            Property(x => x.LoginProvider).IsRequired();
            Property(x => x.ProviderKey).IsRequired();
        }
    }
}
