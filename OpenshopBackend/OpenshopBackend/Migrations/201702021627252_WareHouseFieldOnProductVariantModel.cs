namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WareHouseFieldOnProductVariantModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVariants", "WareHouseCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVariants", "WareHouseCode");
        }
    }
}
