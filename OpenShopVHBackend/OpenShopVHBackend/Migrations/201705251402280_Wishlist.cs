namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Wishlist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WishlistItems",
                c => new
                    {
                        WishlistItemId = c.Int(nullable: false, identity: true),
                        WishlistProductVariantId = c.Int(),
                        DeviceUserId = c.Int(),
                    })
                .PrimaryKey(t => t.WishlistItemId)
                .ForeignKey("dbo.DeviceUsers", t => t.DeviceUserId)
                .ForeignKey("dbo.WishlistProductVariants", t => t.WishlistProductVariantId)
                .Index(t => t.WishlistProductVariantId)
                .Index(t => t.DeviceUserId);
            
            CreateTable(
                "dbo.WishlistProductVariants",
                c => new
                    {
                        WishlistProductVariantId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Name = c.String(),
                        CategoryId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        DiscountPrice = c.Double(nullable: false),
                        PriceFormatted = c.String(),
                        DiscountPriceFormatted = c.String(),
                        Currency = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        MainImage = c.String(),
                        MainImageHighRes = c.String(),
                    })
                .PrimaryKey(t => t.WishlistProductVariantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WishlistItems", "WishlistProductVariantId", "dbo.WishlistProductVariants");
            DropForeignKey("dbo.WishlistItems", "DeviceUserId", "dbo.DeviceUsers");
            DropIndex("dbo.WishlistItems", new[] { "DeviceUserId" });
            DropIndex("dbo.WishlistItems", new[] { "WishlistProductVariantId" });
            DropTable("dbo.WishlistProductVariants");
            DropTable("dbo.WishlistItems");
        }
    }
}
