namespace Auth.FWT.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Add_Telegram_Job_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TelegramJob",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    CreatedDateUTC = c.DateTime(nullable: false),
                    LastStatusUpdateDateUTC = c.DateTime(nullable: false),
                    JobId = c.Guid(nullable: false),
                    Status = c.Int(nullable: false),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.TelegramJob", "UserId", "dbo.User");
            DropIndex("dbo.TelegramJob", new[] { "UserId" });
            DropTable("dbo.TelegramJob");
        }
    }
}