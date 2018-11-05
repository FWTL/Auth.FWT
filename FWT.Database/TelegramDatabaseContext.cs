namespace FWT.Database
{
    using FWT.Database.Configuration;
    using Microsoft.EntityFrameworkCore;

    public class TelegramDatabaseContext : DbContext
    {
        private readonly TelegramDatabaseCredentials _credentials;

        public TelegramDatabaseContext(TelegramDatabaseCredentials credentials)
        {
            _credentials = credentials;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_credentials.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TelegramSessionConfiguration());
        }
    }
}