namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientDiscountModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientDiscounts",
                c => new
                    {
                        ClientDiscountId = c.Int(nullable: false, identity: true),
                        CardCode = c.String(),
                        Discount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ClientDiscountId);
            
            DropTable("dbo.ClientDiscounts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ClientDiscounts",
                c => new
                    {
                        ClientDiscountsId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        CardCode = c.String(),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ClientDiscountsId);
            
            DropTable("dbo.ClientDiscounts");
        }
    }
}
