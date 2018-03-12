using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Auth.FWT.Core.Entities;

namespace Auth.FWT.Data.Configuration
{
    public class TelegramJobConfiguration : EntityTypeConfiguration<TelegramJob>
    {
        public TelegramJobConfiguration()
        {
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasOptional(x => x.TelegramJobData).WithRequired(x => x.TelegramJob);
        }
    }
}