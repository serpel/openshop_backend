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
                        ClientDiscountsId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        CardCode = c.String(),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ClientDiscountsId);
        }
        
        public override void Down()
        {
            DropTable("dbo.ClientDiscounts");
        }
    }
}
