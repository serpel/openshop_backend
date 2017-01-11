namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeOnBrandModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brands", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Brands", "Code");
        }
    }
}
