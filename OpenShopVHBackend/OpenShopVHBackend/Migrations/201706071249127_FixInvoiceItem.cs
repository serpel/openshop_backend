namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixInvoiceItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InvoiceItems", "PaymentId", "dbo.Payments");
            DropIndex("dbo.InvoiceItems", new[] { "PaymentId" });
            AlterColumn("dbo.InvoiceItems", "PaymentId", c => c.Int());
            CreateIndex("dbo.InvoiceItems", "PaymentId");
            AddForeignKey("dbo.InvoiceItems", "PaymentId", "dbo.Payments", "PaymentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceItems", "PaymentId", "dbo.Payments");
            DropIndex("dbo.InvoiceItems", new[] { "PaymentId" });
            AlterColumn("dbo.InvoiceItems", "PaymentId", c => c.Int(nullable: false));
            CreateIndex("dbo.InvoiceItems", "PaymentId");
            AddForeignKey("dbo.InvoiceItems", "PaymentId", "dbo.Payments", "PaymentId", cascadeDelete: true);
        }
    }
}
