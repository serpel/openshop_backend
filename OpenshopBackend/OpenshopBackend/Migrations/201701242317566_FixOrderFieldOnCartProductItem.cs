namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixOrderFieldOnCartProductItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.CartProductItems", new[] { "OrderId" });
            AlterColumn("dbo.CartProductItems", "OrderId", c => c.Int());
            CreateIndex("dbo.CartProductItems", "OrderId");
            AddForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders", "OrderId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.CartProductItems", new[] { "OrderId" });
            AlterColumn("dbo.CartProductItems", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.CartProductItems", "OrderId");
            AddForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
        }
    }
}
