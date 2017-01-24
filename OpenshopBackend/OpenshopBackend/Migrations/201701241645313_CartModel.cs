namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartProductItems",
                c => new
                    {
                        CartProductItemId = c.Int(nullable: false, identity: true),
                        CartId = c.Int(nullable: false),
                        RemoteId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        TotalItemPrice = c.Double(nullable: false),
                        TotalItemPriceFormatted = c.String(),
                        CartProductVariantId = c.Int(nullable: false),
                        Expiration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CartProductItemId)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.CartId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        TotalPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CartId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        RemoteId = c.String(),
                        Status = c.String(),
                        Total = c.Int(nullable: false),
                        Currency = c.String(),
                        CardCode = c.String(),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.CartProductVariants",
                c => new
                    {
                        CartProductVariantId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Url = c.String(),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        PriceFormatted = c.String(),
                        CategoryId = c.Int(nullable: false),
                        MainImage = c.String(),
                        ColorId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CartProductVariantId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Colors", t => t.ColorId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.ColorId)
                .Index(t => t.SizeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartProductVariants", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.CartProductVariants", "ColorId", "dbo.Colors");
            DropForeignKey("dbo.CartProductVariants", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.CartProductItems", "CartId", "dbo.Carts");
            DropIndex("dbo.CartProductVariants", new[] { "SizeId" });
            DropIndex("dbo.CartProductVariants", new[] { "ColorId" });
            DropIndex("dbo.CartProductVariants", new[] { "CategoryId" });
            DropIndex("dbo.CartProductItems", new[] { "OrderId" });
            DropIndex("dbo.CartProductItems", new[] { "CartId" });
            DropTable("dbo.CartProductVariants");
            DropTable("dbo.Orders");
            DropTable("dbo.Carts");
            DropTable("dbo.CartProductItems");
        }
    }
}
