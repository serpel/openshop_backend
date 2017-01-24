namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMigration : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.CartProductItems", "CartProductVariantId");
            AddForeignKey("dbo.CartProductItems", "CartProductVariantId", "dbo.CartProductVariants", "CartProductVariantId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartProductItems", "CartProductVariantId", "dbo.CartProductVariants");
            DropIndex("dbo.CartProductItems", new[] { "CartProductVariantId" });
        }
    }
}
