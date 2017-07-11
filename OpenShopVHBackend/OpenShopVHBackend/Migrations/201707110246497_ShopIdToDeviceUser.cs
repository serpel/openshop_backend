namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopIdToDeviceUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "ShopId", c => c.Int());
            AddColumn("dbo.Shops", "ConnectionString", c => c.String());
            CreateIndex("dbo.DeviceUsers", "ShopId");
            AddForeignKey("dbo.DeviceUsers", "ShopId", "dbo.Shops", "ShopId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeviceUsers", "ShopId", "dbo.Shops");
            DropIndex("dbo.DeviceUsers", new[] { "ShopId" });
            DropColumn("dbo.Shops", "ConnectionString");
            DropColumn("dbo.DeviceUsers", "ShopId");
        }
    }
}
