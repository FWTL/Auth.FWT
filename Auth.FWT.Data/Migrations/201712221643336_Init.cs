namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LockoutEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        SecurityStamp = c.String(maxLength: 36, unicode: false),
                        UserName = c.String(nullable: false, maxLength: 80),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(nullable: false, maxLength: 80),
                        ClaimValue = c.String(nullable: false, maxLength: 80),
                        UserId = c.Int(nullable: false),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false, maxLength: 80),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(nullable: false, maxLength: 80),
                        ProviderKey = c.String(nullable: false, maxLength: 80),
                        UserId = c.Int(nullable: false),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            CreateTable(
                "dbo.ClientAPI",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 80),
                        Active = c.Boolean(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 80),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        Secret = c.String(nullable: false, maxLength: 80),
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
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
                        DeleteDateUTC = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientAPI", t => t.ClientAPIId)
                .Index(t => t.ClientAPIId)
                .Index(t => t.IsDeleted, name: "IX_Deleted");
            
            CreateTable(
                "dbo.UserRoleUser",
                c => new
                    {
                        UserRole_Id = c.Byte(nullable: false),
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
            DropForeignKey("dbo.UserRoleUser", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserRoleUser", "UserRole_Id", "dbo.UserRole");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropIndex("dbo.UserRoleUser", new[] { "User_Id" });
            DropIndex("dbo.UserRoleUser", new[] { "UserRole_Id" });
            DropIndex("dbo.RefreshToken", "IX_Deleted");
            DropIndex("dbo.RefreshToken", new[] { "ClientAPIId" });
            DropIndex("dbo.ClientAPI", "IX_Deleted");
            DropIndex("dbo.UserLogin", "IX_Deleted");
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserRole", "IX_Deleted");
            DropIndex("dbo.UserClaim", "IX_Deleted");
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.User", "IX_Deleted");
            DropTable("dbo.UserRoleUser");
            DropTable("dbo.RefreshToken");
            DropTable("dbo.ClientAPI");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
        }
    }
}
