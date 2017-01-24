namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserToCartModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "DeviceUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Carts", "TotalPriceFormatted", c => c.String());
            AddColumn("dbo.Carts", "Currency", c => c.String());
            CreateIndex("dbo.Carts", "DeviceUserId");
            AddForeignKey("dbo.Carts", "DeviceUserId", "dbo.DeviceUsers", "DeviceUserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carts", "DeviceUserId", "dbo.DeviceUsers");
            DropIndex("dbo.Carts", new[] { "DeviceUserId" });
            DropColumn("dbo.Carts", "Currency");
            DropColumn("dbo.Carts", "TotalPriceFormatted");
            DropColumn("dbo.Carts", "DeviceUserId");
        }
    }
}
