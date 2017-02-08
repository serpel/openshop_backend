namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoggerModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Username = c.String(),
                        Level = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                        Logger = c.String(),
                        CallSite = c.String(),
                        ServerName = c.String(),
                        Port = c.String(),
                        Url = c.String(),
                        RemoteAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogEntries");
        }
    }
}
