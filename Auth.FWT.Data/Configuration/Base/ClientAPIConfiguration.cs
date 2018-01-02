using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Domain.Entities.API;

namespace Auth.FWT.Data.Base.Configuration
{
    public class ClientAPIConfiguration : EntityTypeConfiguration<ClientAPI>
    {
        public ClientAPIConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Secret).IsRequired();
            Property(x => x.AllowedOrigin).HasMaxLength(100);
        }
    }
}
