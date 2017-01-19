namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSizeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sizes", "RemoteId", c => c.Int(nullable: false));
            AddColumn("dbo.Sizes", "Value", c => c.String(nullable: false));
            AddColumn("dbo.Sizes", "Description", c => c.String());
            DropColumn("dbo.Sizes", "Name");
            DropColumn("dbo.Sizes", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sizes", "Code", c => c.String());
            AddColumn("dbo.Sizes", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Sizes", "Description");
            DropColumn("dbo.Sizes", "Value");
            DropColumn("dbo.Sizes", "RemoteId");
        }
    }
}
