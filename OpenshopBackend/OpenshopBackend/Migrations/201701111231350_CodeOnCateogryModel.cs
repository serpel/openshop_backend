namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeOnCateogryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Code");
        }
    }
}
