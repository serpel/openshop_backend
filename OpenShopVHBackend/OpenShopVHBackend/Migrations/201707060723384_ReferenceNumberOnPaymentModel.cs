namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceNumberOnPaymentModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "CollectId", c => c.String());
            AddColumn("dbo.Payments", "ReferenceNumber", c => c.String());
            DropColumn("dbo.DeviceUsers", "DebtCollerctor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeviceUsers", "DebtCollerctor", c => c.String());
            DropColumn("dbo.Payments", "ReferenceNumber");
            DropColumn("dbo.DeviceUsers", "CollectId");
        }
    }
}
