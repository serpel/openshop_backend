namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesPersonNumberOnDeviceUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "SalesPersonId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceUsers", "SalesPersonId");
        }
    }
}
