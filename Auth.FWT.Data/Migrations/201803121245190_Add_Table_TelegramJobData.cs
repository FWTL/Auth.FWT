namespace Auth.FWT.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Add_Table_TelegramJobData : DbMigration
    {
        public override void Down()
        {
            DropForeignKey("dbo.TelegramJobData", "Id", "dbo.TelegramJob");
            DropIndex("dbo.TelegramJobData", new[] { "Id" });
            DropTable("dbo.TelegramJobData");
        }

        public override void Up()
        {
            CreateTable(
                "dbo.TelegramJobData",
                c => new
                {
                    Id = c.Long(nullable: false),
                    Data = c.Binary(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TelegramJob", t => t.Id)
                .Index(t => t.Id);
        }
    }
}
