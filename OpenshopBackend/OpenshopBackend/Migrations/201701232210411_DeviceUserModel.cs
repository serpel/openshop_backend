namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceUserModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceUsers",
                c => new
                    {
                        DeviceUserId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AccessToken = c.String(),
                    })
                .PrimaryKey(t => t.DeviceUserId);
            
            DropTable("dbo.AppUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        AppUserId = c.Int(nullable: false, identity: true),
                        fbId = c.String(),
                        Name = c.String(),
                        AccessToken = c.String(),
                    })
                .PrimaryKey(t => t.AppUserId);
            
            DropTable("dbo.DeviceUsers");
        }
    }
}
