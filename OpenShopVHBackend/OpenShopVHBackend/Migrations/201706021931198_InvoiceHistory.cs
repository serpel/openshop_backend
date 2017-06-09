namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceHistories",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        DocNum = c.String(),
                        CardCode = c.String(),
                        CardName = c.String(),
                        Total = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.InvoiceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InvoiceHistories");
        }
    }
}
