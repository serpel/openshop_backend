namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductModelRemoveCollections : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductVariants", "Product_ProductId", "dbo.Products");
            DropIndex("dbo.Products", new[] { "Product_ProductId" });
            DropIndex("dbo.ProductVariants", new[] { "Product_ProductId" });
            DropColumn("dbo.Products", "Product_ProductId");
            DropColumn("dbo.ProductVariants", "Product_ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductVariants", "Product_ProductId", c => c.Int());
            AddColumn("dbo.Products", "Product_ProductId", c => c.Int());
            CreateIndex("dbo.ProductVariants", "Product_ProductId");
            CreateIndex("dbo.Products", "Product_ProductId");
            AddForeignKey("dbo.ProductVariants", "Product_ProductId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.Products", "Product_ProductId", "dbo.Products", "ProductId");
        }
    }
}
