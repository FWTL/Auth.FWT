namespace Auth.FWT.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class telegramcodehash : DbMigration
    {
        public override void Down()
        {
            DropTable("dbo.TelegramCode");
        }

        public override void Up()
        {
            CreateTable(
                "dbo.TelegramCode",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 80),
                    CodeHash = c.String(nullable: false, maxLength: 80),
                })
                .PrimaryKey(t => t.Id);
        }
    }
}
