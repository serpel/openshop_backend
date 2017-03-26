namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdFieldToCategoryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Id");
        }
    }
}
