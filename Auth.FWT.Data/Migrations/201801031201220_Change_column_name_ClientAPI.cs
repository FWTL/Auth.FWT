namespace Auth.FWT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_column_name_ClientAPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientAPI", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.ClientAPI", "Active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClientAPI", "Active", c => c.Boolean(nullable: false));
            DropColumn("dbo.ClientAPI", "IsActive");
        }
    }
}
