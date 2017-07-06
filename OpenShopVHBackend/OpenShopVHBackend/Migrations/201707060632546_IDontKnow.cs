namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IDontKnow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "DebtCollerctor", c => c.String());
            DropColumn("dbo.DeviceUsers", "CollectId");
            DropColumn("dbo.Payments", "ReferenceNumber");
            DropTable("dbo.FilterTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FilterTypes",
                c => new
                    {
                        FilterTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.FilterTypeId);
            
            AddColumn("dbo.Payments", "ReferenceNumber", c => c.String());
            AddColumn("dbo.DeviceUsers", "CollectId", c => c.String());
            DropColumn("dbo.DeviceUsers", "DebtCollerctor");
        }
    }
}
