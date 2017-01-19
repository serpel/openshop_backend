namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRequiredFieldsOnProductVariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVariants", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.ProductVariants", "IsCommitted", c => c.Int(nullable: false));
            AddColumn("dbo.ProductVariants", "Price", c => c.Double(nullable: false));
            AddColumn("dbo.ProductVariants", "Currency", c => c.String());
            DropColumn("dbo.Products", "Quantity");
            DropColumn("dbo.Products", "IsCommitted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "IsCommitted", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Quantity", c => c.Int(nullable: false));
            DropColumn("dbo.ProductVariants", "Currency");
            DropColumn("dbo.ProductVariants", "Price");
            DropColumn("dbo.ProductVariants", "IsCommitted");
            DropColumn("dbo.ProductVariants", "Quantity");
        }
    }
}
