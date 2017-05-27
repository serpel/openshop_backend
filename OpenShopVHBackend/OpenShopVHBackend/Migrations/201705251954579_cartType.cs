namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartProductItems", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Carts", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "Type");
            DropColumn("dbo.CartProductItems", "Type");
        }
    }
}
