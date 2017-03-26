namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryFixModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Categories", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Categories", "Code", c => c.String());
        }
    }
}
