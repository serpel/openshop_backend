namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedFieldToProductVariant : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Code", c => c.String(nullable: false));
            DropColumn("dbo.Products", "Price");
            DropColumn("dbo.Products", "PriceFormated");
            DropColumn("dbo.Products", "DisountedPrice");
            DropColumn("dbo.Products", "DisountedPriceFormated");
            DropColumn("dbo.Products", "Currency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Currency", c => c.String(nullable: false));
            AddColumn("dbo.Products", "DisountedPriceFormated", c => c.String());
            AddColumn("dbo.Products", "DisountedPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Products", "PriceFormated", c => c.String());
            AddColumn("dbo.Products", "Price", c => c.Double(nullable: false));
            AlterColumn("dbo.Products", "Code", c => c.String());
        }
    }
}
