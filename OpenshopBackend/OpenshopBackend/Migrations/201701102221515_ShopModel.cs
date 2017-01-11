namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        ShopId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Url = c.String(),
                        Logo = c.String(),
                        GoogleUA = c.String(),
                        Language = c.String(),
                        Currency = c.String(),
                        FlagIcon = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shops");
        }
    }
}
