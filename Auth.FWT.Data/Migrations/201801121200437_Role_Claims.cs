namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Role_Claims : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(maxLength: 80),
                        ClaimValue = c.String(maxLength: 80),
                        RoleId = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRole", t => t.RoleId)
                .Index(t => t.RoleId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleClaim", "RoleId", "dbo.UserRole");
            DropIndex("dbo.RoleClaim", new[] { "RoleId" });
            DropTable("dbo.RoleClaim");
        }
    }
}
