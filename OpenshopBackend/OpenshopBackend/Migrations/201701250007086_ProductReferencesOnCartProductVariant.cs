namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductReferencesOnCartProductVariant : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CartProductVariants", "ProductId", c => c.Int());
            CreateIndex("dbo.CartProductVariants", "ProductId");
            AddForeignKey("dbo.CartProductVariants", "ProductId", "dbo.Products", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartProductVariants", "ProductId", "dbo.Products");
            DropIndex("dbo.CartProductVariants", new[] { "ProductId" });
            AlterColumn("dbo.CartProductVariants", "ProductId", c => c.Int(nullable: false));
        }
    }
}
