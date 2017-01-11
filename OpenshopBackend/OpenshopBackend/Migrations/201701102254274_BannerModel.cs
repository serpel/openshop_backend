namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BannerModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banners",
                c => new
                    {
                        BannerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Target = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.BannerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Banners");
        }
    }
}
