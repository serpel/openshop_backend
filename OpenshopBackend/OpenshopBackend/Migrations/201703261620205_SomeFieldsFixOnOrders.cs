namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeFieldsFixOnOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ClientId", c => c.Int());
            AddColumn("dbo.Orders", "DeviceUserId", c => c.Int());
            AddColumn("dbo.OrderItems", "Discount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderItems", "TaxValue", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "RTN", c => c.String());
            CreateIndex("dbo.Orders", "ClientId");
            CreateIndex("dbo.Orders", "DeviceUserId");
            AddForeignKey("dbo.Orders", "ClientId", "dbo.Clients", "ClientId");
            AddForeignKey("dbo.Orders", "DeviceUserId", "dbo.DeviceUsers", "DeviceUserId");
            DropColumn("dbo.Orders", "Total");
            DropColumn("dbo.Orders", "CardCode");
            DropColumn("dbo.Orders", "SalesPersonCode");
            DropColumn("dbo.Clients", "DiscountPercent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "DiscountPercent", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "SalesPersonCode", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "CardCode", c => c.String());
            AddColumn("dbo.Orders", "Total", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "DeviceUserId", "dbo.DeviceUsers");
            DropForeignKey("dbo.Orders", "ClientId", "dbo.Clients");
            DropIndex("dbo.Orders", new[] { "DeviceUserId" });
            DropIndex("dbo.Orders", new[] { "ClientId" });
            DropColumn("dbo.Clients", "RTN");
            DropColumn("dbo.OrderItems", "TaxValue");
            DropColumn("dbo.OrderItems", "Discount");
            DropColumn("dbo.Orders", "DeviceUserId");
            DropColumn("dbo.Orders", "ClientId");
        }
    }
}
