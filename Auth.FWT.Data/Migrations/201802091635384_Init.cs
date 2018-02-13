namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TelegramCode",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 80),
                        CodeHash = c.String(nullable: false, maxLength: 80),
                        IssuedUTC = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TelegramSession",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ExpireDateUtc = c.DateTime(nullable: false),
                        Session = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LockoutEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        SecurityStamp = c.String(maxLength: 36, unicode: false),
                        UserName = c.String(nullable: false, maxLength: 80),
                        PhoneNumberHashed = c.String(maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(nullable: false, maxLength: 80),
                        ClaimValue = c.String(nullable: false, maxLength: 80),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(maxLength: 80),
                        ClaimValue = c.String(maxLength: 80),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRole", t => t.RoleId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(nullable: false, maxLength: 80),
                        ProviderKey = c.String(nullable: false, maxLength: 80),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ClientAPI",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 80),
                        AllowedOrigin = c.String(maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 80),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        Secret = c.String(nullable: false, maxLength: 80),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshToken",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 80),
                        ClientAPIId = c.String(nullable: false, maxLength: 80),
                        ExpiresUtc = c.DateTime(nullable: false),
                        IssuedUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientAPI", t => t.ClientAPIId)
                .Index(t => t.ClientAPIId);
            
            CreateTable(
                "dbo.UserRoleUser",
                c => new
                    {
                        UserRole_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserRole_Id, t.User_Id })
                .ForeignKey("dbo.UserRole", t => t.UserRole_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.UserRole_Id)
                .Index(t => t.User_Id);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RefreshToken", "ClientAPIId", "dbo.ClientAPI");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.TelegramSession", "Id", "dbo.User");
            DropForeignKey("dbo.UserRoleUser", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserRoleUser", "UserRole_Id", "dbo.UserRole");
            DropForeignKey("dbo.RoleClaim", "RoleId", "dbo.UserRole");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropIndex("dbo.UserRoleUser", new[] { "User_Id" });
            DropIndex("dbo.UserRoleUser", new[] { "UserRole_Id" });
            DropIndex("dbo.RefreshToken", new[] { "ClientAPIId" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.RoleClaim", new[] { "RoleId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.TelegramSession", new[] { "Id" });
            DropTable("dbo.UserRoleUser");
            DropTable("dbo.RefreshToken");
            DropTable("dbo.ClientAPI");
            DropTable("dbo.UserLogin");
            DropTable("dbo.RoleClaim");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
            DropTable("dbo.TelegramSession");
            DropTable("dbo.TelegramCode");
        }
    }
}
