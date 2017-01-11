namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BrandImg = c.String(),
                        IsPremium = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BrandId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        RemoteId = c.Int(nullable: false),
                        PartentId = c.Int(nullable: false),
                        Name = c.String(),
                        Type = c.String(),
                        Category_CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Categories", t => t.Category_CategoryId)
                .Index(t => t.Category_CategoryId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        RemoteId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                        Price = c.Double(nullable: false),
                        PriceFormated = c.String(),
                        DisountedPrice = c.Double(nullable: false),
                        DisountedPriceFormated = c.String(),
                        CategoryId = c.Int(nullable: false),
                        BrandId = c.Int(nullable: false),
                        Currency = c.String(nullable: false),
                        Description = c.String(),
                        MainImage = c.String(),
                        MainImageHighRes = c.String(),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.CategoryId)
                .Index(t => t.BrandId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.ProductVariants",
                c => new
                    {
                        ProductVariantId = c.Int(nullable: false, identity: true),
                        ColorId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        Code = c.String(),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductVariantId)
                .ForeignKey("dbo.Colors", t => t.ColorId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.ColorId)
                .Index(t => t.SizeId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        ColorId = c.Int(nullable: false, identity: true),
                        RemoteId = c.Int(nullable: false),
                        Value = c.String(),
                        Code = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.ColorId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        SizeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.SizeId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductVariants", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductVariants", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.ProductVariants", "ColorId", "dbo.Colors");
            DropForeignKey("dbo.Products", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Products", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.Categories", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProductVariants", new[] { "Product_ProductId" });
            DropIndex("dbo.ProductVariants", new[] { "SizeId" });
            DropIndex("dbo.ProductVariants", new[] { "ColorId" });
            DropIndex("dbo.Products", new[] { "Product_ProductId" });
            DropIndex("dbo.Products", new[] { "BrandId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Categories", new[] { "Category_CategoryId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Sizes");
            DropTable("dbo.Colors");
            DropTable("dbo.ProductVariants");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
            DropTable("dbo.Brands");
        }
    }
}
