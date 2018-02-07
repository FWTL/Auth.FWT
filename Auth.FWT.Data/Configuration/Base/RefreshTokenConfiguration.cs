using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.Entities.API;

namespace Auth.FWT.Data.Base.Configuration
{
    public class RefreshTokenConfiguration : EntityTypeConfiguration<RefreshToken>
    {
        public RefreshTokenConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Subject).HasMaxLength(50).IsRequired();
            Property(x => x.ClientAPIId).IsRequired();
            Property(x => x.ProtectedTicket).IsRequired().IsMaxLength();
            HasKey(x => x.Id);
        }
    }
}
