namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtraFieldsProductsModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Season", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Season");
        }
    }
}
