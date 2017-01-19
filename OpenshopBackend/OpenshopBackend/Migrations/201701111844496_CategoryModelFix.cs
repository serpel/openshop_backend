namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryModelFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Categories", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.Categories", new[] { "Category_CategoryId" });
            DropColumn("dbo.Categories", "Category_CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Categories", "Category_CategoryId", c => c.Int());
            CreateIndex("dbo.Categories", "Category_CategoryId");
            AddForeignKey("dbo.Categories", "Category_CategoryId", "dbo.Categories", "CategoryId");
        }
    }
}
