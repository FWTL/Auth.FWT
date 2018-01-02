namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Soft_Deletes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.User", "IX_Deleted");
            DropIndex("dbo.UserClaim", "IX_Deleted");
            DropIndex("dbo.UserRole", "IX_Deleted");
            DropIndex("dbo.TelegramSession", "IX_Deleted");
            DropIndex("dbo.UserLogin", "IX_Deleted");
            DropIndex("dbo.ClientAPI", "IX_Deleted");
            DropIndex("dbo.RefreshToken", "IX_Deleted");
            DropColumn("dbo.User", "DeleteDateUTC");
            DropColumn("dbo.User", "IsDeleted");
            DropColumn("dbo.UserClaim", "DeleteDateUTC");
            DropColumn("dbo.UserClaim", "IsDeleted");
            DropColumn("dbo.UserRole", "DeleteDateUTC");
            DropColumn("dbo.UserRole", "IsDeleted");
            DropColumn("dbo.TelegramSession", "DeleteDateUTC");
            DropColumn("dbo.TelegramSession", "IsDeleted");
            DropColumn("dbo.UserLogin", "DeleteDateUTC");
            DropColumn("dbo.UserLogin", "IsDeleted");
            DropColumn("dbo.ClientAPI", "DeleteDateUTC");
            DropColumn("dbo.ClientAPI", "IsDeleted");
            DropColumn("dbo.RefreshToken", "DeleteDateUTC");
            DropColumn("dbo.RefreshToken", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RefreshToken", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.RefreshToken", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.ClientAPI", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ClientAPI", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.UserLogin", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserLogin", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.TelegramSession", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.TelegramSession", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.UserRole", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserRole", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.UserClaim", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserClaim", "DeleteDateUTC", c => c.DateTime());
            AddColumn("dbo.User", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "DeleteDateUTC", c => c.DateTime());
            CreateIndex("dbo.RefreshToken", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.ClientAPI", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.UserLogin", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.TelegramSession", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.UserRole", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.UserClaim", "IsDeleted", name: "IX_Deleted");
            CreateIndex("dbo.User", "IsDeleted", name: "IX_Deleted");
        }
    }
}
