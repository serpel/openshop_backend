namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixUserModel : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AppUsers");
            AlterColumn("dbo.AppUsers", "AppUserId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.AppUsers", "AppUserId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AppUsers");
            AlterColumn("dbo.AppUsers", "AppUserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.AppUsers", "AppUserId");
        }
    }
}
