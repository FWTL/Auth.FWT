namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableTelegramSession : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TelegramSession",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Session = c.Binary(),
                        UserId = c.Int(nullable: false),
                        ExpireDateUtc = c.DateTime(nullable: false),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            AddColumn("dbo.User", "TelegramSessionId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramSession", "Id", "dbo.User");
            DropIndex("dbo.TelegramSession", "IX_Deleted");
            DropIndex("dbo.TelegramSession", new[] { "Id" });
            DropColumn("dbo.User", "TelegramSessionId");
            DropTable("dbo.TelegramSession");
        }
    }
}
