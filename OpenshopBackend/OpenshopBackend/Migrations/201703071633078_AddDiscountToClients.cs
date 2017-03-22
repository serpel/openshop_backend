namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDiscountToClients : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "DiscountPercent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "DiscountPercent");
        }
    }
}
