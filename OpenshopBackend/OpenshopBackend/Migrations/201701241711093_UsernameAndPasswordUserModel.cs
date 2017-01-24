namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsernameAndPasswordUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "Username", c => c.String());
            AddColumn("dbo.DeviceUsers", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceUsers", "Password");
            DropColumn("dbo.DeviceUsers", "Username");
        }
    }
}
