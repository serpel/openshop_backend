namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientDocumentsModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        DocumentCode = c.String(),
                        CreatedDate = c.String(),
                        DueDate = c.String(),
                        TotalAmount = c.Double(nullable: false),
                        PayedAmount = c.Double(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "ClientId", "dbo.Clients");
            DropIndex("dbo.Documents", new[] { "ClientId" });
            DropTable("dbo.Documents");
        }
    }
}
