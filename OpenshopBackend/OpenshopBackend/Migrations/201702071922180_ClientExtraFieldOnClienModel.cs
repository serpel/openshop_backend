namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientExtraFieldOnClienModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "CreditLimit", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "Balance", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "InOrders", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "PayCondition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "PayCondition");
            DropColumn("dbo.Clients", "InOrders");
            DropColumn("dbo.Clients", "Balance");
            DropColumn("dbo.Clients", "CreditLimit");
        }
    }
}
