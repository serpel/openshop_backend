namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProducReferenceOnProducVariantModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVariants", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductVariants", "Code", c => c.String(nullable: false));
            CreateIndex("dbo.ProductVariants", "ProductId");
            AddForeignKey("dbo.ProductVariants", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductVariants", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductVariants", new[] { "ProductId" });
            AlterColumn("dbo.ProductVariants", "Code", c => c.String());
            DropColumn("dbo.ProductVariants", "ProductId");
        }
    }
}
