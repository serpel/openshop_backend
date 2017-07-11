namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ISVFieldOnShopModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "ISV", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "ISV");
        }
    }
}
