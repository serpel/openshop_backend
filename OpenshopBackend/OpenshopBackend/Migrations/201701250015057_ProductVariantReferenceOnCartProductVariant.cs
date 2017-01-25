namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductVariantReferenceOnCartProductVariant : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartProductVariants", "ProductId", "dbo.Products");
            DropIndex("dbo.CartProductVariants", new[] { "ProductId" });
            AddColumn("dbo.CartProductVariants", "ProductVariantId", c => c.Int());
            CreateIndex("dbo.CartProductVariants", "ProductVariantId");
            AddForeignKey("dbo.CartProductVariants", "ProductVariantId", "dbo.ProductVariants", "ProductVariantId");
            DropColumn("dbo.CartProductVariants", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartProductVariants", "ProductId", c => c.Int());
            DropForeignKey("dbo.CartProductVariants", "ProductVariantId", "dbo.ProductVariants");
            DropIndex("dbo.CartProductVariants", new[] { "ProductVariantId" });
            DropColumn("dbo.CartProductVariants", "ProductVariantId");
            CreateIndex("dbo.CartProductVariants", "ProductId");
            AddForeignKey("dbo.CartProductVariants", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
