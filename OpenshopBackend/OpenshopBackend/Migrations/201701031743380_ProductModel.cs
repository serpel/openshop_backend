namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BrandImg = c.String(),
                        IsPremium = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BrandId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        RemoteId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                        Price = c.Double(nullable: false),
                        PriceFormated = c.String(),
                        DisountedPrice = c.Double(nullable: false),
                        DisountedPriceFormated = c.String(),
                        CategoryId = c.Int(nullable: false),
                        BrandId = c.Int(nullable: false),
                        Currency = c.String(nullable: false),
                        Description = c.String(),
                        MainImage = c.String(),
                        MainImageHighRes = c.String(),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.CategoryId)
                .Index(t => t.BrandId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.ProductVariants",
                c => new
                    {
                        ProductVariantId = c.Int(nullable: false, identity: true),
                        ColorId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        Code = c.String(),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductVariantId)
                .ForeignKey("dbo.Colors", t => t.ColorId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.ColorId)
                .Index(t => t.SizeId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        ColorId = c.Int(nullable: false, identity: true),
                        RemoteId = c.Int(nullable: false),
                        Value = c.String(),
                        Code = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.ColorId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        SizeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.SizeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductVariants", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductVariants", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.ProductVariants", "ColorId", "dbo.Colors");
            DropForeignKey("dbo.Products", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Products", "BrandId", "dbo.Brands");
            DropIndex("dbo.ProductVariants", new[] { "Product_ProductId" });
            DropIndex("dbo.ProductVariants", new[] { "SizeId" });
            DropIndex("dbo.ProductVariants", new[] { "ColorId" });
            DropIndex("dbo.Products", new[] { "Product_ProductId" });
            DropIndex("dbo.Products", new[] { "BrandId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.Sizes");
            DropTable("dbo.Colors");
            DropTable("dbo.ProductVariants");
            DropTable("dbo.Products");
            DropTable("dbo.Brands");
        }
    }
}
