namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderItemModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        SKU = c.String(),
                        Quantity = c.Int(nullable: false),
                        WarehouseCode = c.String(),
                        TaxCode = c.String(),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            AddColumn("dbo.Orders", "DateCreated", c => c.String());
            AddColumn("dbo.Orders", "Comment", c => c.String());
            AddColumn("dbo.Orders", "SalesPersonCode", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "Series", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "Currency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Currency", c => c.String());
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropColumn("dbo.Orders", "Series");
            DropColumn("dbo.Orders", "SalesPersonCode");
            DropColumn("dbo.Orders", "Comment");
            DropColumn("dbo.Orders", "DateCreated");
            DropTable("dbo.OrderItems");
        }
    }
}
