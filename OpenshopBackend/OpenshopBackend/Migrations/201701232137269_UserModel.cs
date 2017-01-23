namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        fbId = c.String(),
                        Name = c.String(),
                        AccessToken = c.String(),
                    })
                .PrimaryKey(t => t.AppUserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AppUsers");
        }
    }
}
