namespace OpenShopVHBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banks",
                c => new
                {
                    BankId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    FormatCode = c.String(),
                    GeneralAccount = c.String(),
                })
                .PrimaryKey(t => t.BankId);

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

            CreateTable(
                "dbo.Brands",
                c => new
                {
                    BrandId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Code = c.String(),
                    BrandImg = c.String(),
                    IsPremium = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.BrandId);

            CreateTable(
                "dbo.CartProductItems",
                c => new
                {
                    CartProductItemId = c.Int(nullable: false, identity: true),
                    CartProductVariantId = c.Int(nullable: false),
                    CartId = c.Int(nullable: false),
                    RemoteId = c.Int(nullable: false),
                    OrderId = c.Int(),
                    Quantity = c.Int(nullable: false),
                    Discount = c.Double(nullable: false),
                    DiscountPercent = c.Double(nullable: false),
                    ISV = c.Double(nullable: false),
                    TotalItemPrice = c.Double(nullable: false),
                    TotalItemPriceFormatted = c.String(),
                    Expiration = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.CartProductItemId)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.CartProductVariants", t => t.CartProductVariantId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.CartProductVariantId)
                .Index(t => t.CartId)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.Carts",
                c => new
                {
                    CartId = c.Int(nullable: false, identity: true),
                    DeviceUserId = c.Int(nullable: false),
                    TotalPrice = c.Double(nullable: false),
                    TotalPriceFormatted = c.String(),
                    Currency = c.String(),
                })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.DeviceUsers", t => t.DeviceUserId, cascadeDelete: true)
                .Index(t => t.DeviceUserId);

            CreateTable(
                "dbo.DeviceUsers",
                c => new
                {
                    DeviceUserId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    SalesPersonId = c.Int(nullable: false),
                    Username = c.String(),
                    Password = c.String(),
                    AccessToken = c.String(),
                    PrintBluetoothAddress = c.String(),
                    DebtCollerctor = c.String(),
                })
                .PrimaryKey(t => t.DeviceUserId);

            CreateTable(
                "dbo.Orders",
                c => new
                {
                    OrderId = c.Int(nullable: false, identity: true),
                    RemoteId = c.String(),
                    DateCreated = c.String(),
                    Status = c.Int(nullable: false),
                    ClientId = c.Int(),
                    DeviceUserId = c.Int(),
                    Comment = c.String(),
                    LastErrorMessage = c.String(),
                    Series = c.Int(nullable: false),
                    DeliveryDate = c.String(),
                })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.DeviceUsers", t => t.DeviceUserId)
                .Index(t => t.ClientId)
                .Index(t => t.DeviceUserId);

            CreateTable(
                "dbo.Clients",
                c => new
                {
                    ClientId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    CardCode = c.String(),
                    PhoneNumber = c.String(),
                    CreditLimit = c.Double(nullable: false),
                    Balance = c.Double(nullable: false),
                    InOrders = c.Double(nullable: false),
                    PayCondition = c.String(),
                    Address = c.String(),
                    RTN = c.String(),
                    PastDue = c.Double(nullable: false),
                    ContactPerson = c.String(),
                })
                .PrimaryKey(t => t.ClientId);

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
                    BalanceDue = c.Double(nullable: false),
                    ClientId = c.Int(nullable: false),
                    DocEntry = c.Int(nullable: false),
                    OverdueDays = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);

            CreateTable(
                "dbo.OrderItems",
                c => new
                {
                    OrderItemId = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    SKU = c.String(),
                    Quantity = c.Int(nullable: false),
                    Price = c.Double(nullable: false),
                    Discount = c.Double(nullable: false),
                    DiscountPercent = c.Double(nullable: false),
                    TaxValue = c.Double(nullable: false),
                    TaxCode = c.String(),
                    WarehouseCode = c.String(),
                })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.CartProductVariants",
                c => new
                {
                    CartProductVariantId = c.Int(nullable: false, identity: true),
                    ProductVariantId = c.Int(),
                    Url = c.String(),
                    Name = c.String(),
                    Price = c.Double(nullable: false),
                    Discount = c.Double(nullable: false),
                    DiscountPercent = c.Double(nullable: false),
                    PriceFormatted = c.String(),
                    CategoryId = c.Int(nullable: false),
                    MainImage = c.String(),
                    WareHouseCode = c.String(),
                    ColorId = c.Int(nullable: false),
                    SizeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.CartProductVariantId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Colors", t => t.ColorId, cascadeDelete: true)
                .ForeignKey("dbo.ProductVariants", t => t.ProductVariantId)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.ProductVariantId)
                .Index(t => t.CategoryId)
                .Index(t => t.ColorId)
                .Index(t => t.SizeId);

            CreateTable(
                "dbo.Categories",
                c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    Id = c.Int(nullable: false),
                    PartentId = c.Int(nullable: false),
                    RemoteId = c.Int(nullable: false),
                    Name = c.String(),
                    Type = c.String(),
                })
                .PrimaryKey(t => t.CategoryId);

            CreateTable(
                "dbo.Colors",
                c => new
                {
                    ColorId = c.Int(nullable: false, identity: true),
                    RemoteId = c.Int(nullable: false),
                    Value = c.String(),
                    Code = c.String(),
                    Image = c.String(),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.ColorId);

            CreateTable(
                "dbo.ProductVariants",
                c => new
                {
                    ProductVariantId = c.Int(nullable: false, identity: true),
                    ItemGroup = c.Int(nullable: false),
                    ProductId = c.Int(nullable: false),
                    ColorId = c.Int(nullable: false),
                    SizeId = c.Int(nullable: false),
                    Code = c.String(nullable: false),
                    Quantity = c.Int(nullable: false),
                    IsCommitted = c.Int(nullable: false),
                    Price = c.Double(nullable: false),
                    Currency = c.String(),
                    WareHouseCode = c.String(),
                })
                .PrimaryKey(t => t.ProductVariantId)
                .ForeignKey("dbo.Colors", t => t.ColorId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ColorId)
                .Index(t => t.SizeId);

            CreateTable(
                "dbo.Products",
                c => new
                {
                    ProductId = c.Int(nullable: false, identity: true),
                    RemoteId = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                    Code = c.String(nullable: false),
                    CategoryId = c.Int(nullable: false),
                    BrandId = c.Int(nullable: false),
                    Season = c.String(),
                    Description = c.String(),
                    MainImage = c.String(),
                    MainImageHighRes = c.String(),
                })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.BrandId);

            CreateTable(
                "dbo.Sizes",
                c => new
                {
                    SizeId = c.Int(nullable: false, identity: true),
                    RemoteId = c.Int(nullable: false),
                    Value = c.String(nullable: false),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.SizeId);

            CreateTable(
                "dbo.Cashes",
                c => new
                {
                    CashId = c.Int(nullable: false, identity: true),
                    GeneralAccount = c.String(),
                    Amount = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.CashId);

            CreateTable(
                "dbo.Checks",
                c => new
                {
                    CheckId = c.Int(nullable: false, identity: true),
                    PaymentId = c.Int(nullable: false),
                    RefenceNumber = c.String(),
                    BankId = c.Int(nullable: false),
                    Amount = c.Double(nullable: false),
                    GeneralAccount = c.String(),
                    DueDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Banks", t => t.BankId, cascadeDelete: true)
                .ForeignKey("dbo.Payments", t => t.PaymentId, cascadeDelete: true)
                .Index(t => t.PaymentId)
                .Index(t => t.BankId);

            CreateTable(
                "dbo.Payments",
                c => new
                {
                    PaymentId = c.Int(nullable: false, identity: true),
                    DocEntry = c.String(),
                    ClientId = c.Int(),
                    CashId = c.Int(),
                    TransferId = c.Int(),
                    DeviceUserId = c.Int(),
                    TotalAmount = c.Double(nullable: false),
                    LastErrorMessage = c.String(),
                    Status = c.Int(nullable: false),
                    Comment = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Cashes", t => t.CashId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.DeviceUsers", t => t.DeviceUserId)
                .ForeignKey("dbo.Transfers", t => t.TransferId)
                .Index(t => t.ClientId)
                .Index(t => t.CashId)
                .Index(t => t.TransferId)
                .Index(t => t.DeviceUserId);

            CreateTable(
                "dbo.InvoiceItems",
                c => new
                {
                    InvoiceItemId = c.Int(nullable: false, identity: true),
                    PaymentId = c.Int(nullable: false),
                    DocEntry = c.Int(nullable: false),
                    DocumentNumber = c.String(),
                    TotalAmount = c.Double(nullable: false),
                    PayedAmount = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.InvoiceItemId)
                .ForeignKey("dbo.Payments", t => t.PaymentId, cascadeDelete: true)
                .Index(t => t.PaymentId);

            CreateTable(
                "dbo.Transfers",
                c => new
                {
                    TransferId = c.Int(nullable: false, identity: true),
                    ReferenceNumber = c.String(),
                    Amount = c.Double(nullable: false),
                    Date = c.DateTime(nullable: false),
                    GeneralAccount = c.String(),
                })
                .PrimaryKey(t => t.TransferId);

            CreateTable(
                "dbo.ClientDiscounts",
                c => new
                {
                    ClientDiscountId = c.Int(nullable: false, identity: true),
                    CardCode = c.String(),
                    ItemGroup = c.Int(nullable: false),
                    Discount = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.ClientDiscountId);

            CreateTable(
                "dbo.ClientTransactions",
                c => new
                {
                    ClientTransactionId = c.Int(nullable: false, identity: true),
                    ReferenceNumber = c.Int(nullable: false),
                    CardCode = c.String(),
                    Description = c.String(),
                    Amount = c.Double(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ClientTransactionId);

            CreateTable(
                "dbo.Devices",
                c => new
                {
                    DeviceId = c.Int(nullable: false, identity: true),
                    DeviceToken = c.String(),
                    Platform = c.String(),
                })
                .PrimaryKey(t => t.DeviceId);

            CreateTable(
                "dbo.Fees",
                c => new
                {
                    FeesId = c.Int(nullable: false, identity: true),
                    DeviceUserId = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false),
                    Amount = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.FeesId)
                .ForeignKey("dbo.DeviceUsers", t => t.DeviceUserId, cascadeDelete: true)
                .Index(t => t.DeviceUserId);

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
                "dbo.Settings",
                c => new
                {
                    SettingId = c.Int(nullable: false, identity: true),
                    ShopId = c.Int(nullable: false),
                    Currency = c.String(),
                    ISV = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.SettingId)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ShopId);

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

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    ProfileUrl = c.String(),
                    Culture = c.String(),
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
            DropForeignKey("dbo.Settings", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Fees", "DeviceUserId", "dbo.DeviceUsers");
            DropForeignKey("dbo.Payments", "TransferId", "dbo.Transfers");
            DropForeignKey("dbo.InvoiceItems", "PaymentId", "dbo.Payments");
            DropForeignKey("dbo.Payments", "DeviceUserId", "dbo.DeviceUsers");
            DropForeignKey("dbo.Payments", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Checks", "PaymentId", "dbo.Payments");
            DropForeignKey("dbo.Payments", "CashId", "dbo.Cashes");
            DropForeignKey("dbo.Checks", "BankId", "dbo.Banks");
            DropForeignKey("dbo.CartProductItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.CartProductItems", "CartProductVariantId", "dbo.CartProductVariants");
            DropForeignKey("dbo.CartProductVariants", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.CartProductVariants", "ProductVariantId", "dbo.ProductVariants");
            DropForeignKey("dbo.ProductVariants", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.ProductVariants", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Products", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.ProductVariants", "ColorId", "dbo.Colors");
            DropForeignKey("dbo.CartProductVariants", "ColorId", "dbo.Colors");
            DropForeignKey("dbo.CartProductVariants", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Carts", "DeviceUserId", "dbo.DeviceUsers");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "DeviceUserId", "dbo.DeviceUsers");
            DropForeignKey("dbo.Orders", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Documents", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.CartProductItems", "CartId", "dbo.Carts");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Settings", new[] { "ShopId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Fees", new[] { "DeviceUserId" });
            DropIndex("dbo.InvoiceItems", new[] { "PaymentId" });
            DropIndex("dbo.Payments", new[] { "DeviceUserId" });
            DropIndex("dbo.Payments", new[] { "TransferId" });
            DropIndex("dbo.Payments", new[] { "CashId" });
            DropIndex("dbo.Payments", new[] { "ClientId" });
            DropIndex("dbo.Checks", new[] { "BankId" });
            DropIndex("dbo.Checks", new[] { "PaymentId" });
            DropIndex("dbo.Products", new[] { "BrandId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.ProductVariants", new[] { "SizeId" });
            DropIndex("dbo.ProductVariants", new[] { "ColorId" });
            DropIndex("dbo.ProductVariants", new[] { "ProductId" });
            DropIndex("dbo.CartProductVariants", new[] { "SizeId" });
            DropIndex("dbo.CartProductVariants", new[] { "ColorId" });
            DropIndex("dbo.CartProductVariants", new[] { "CategoryId" });
            DropIndex("dbo.CartProductVariants", new[] { "ProductVariantId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.Documents", new[] { "ClientId" });
            DropIndex("dbo.Orders", new[] { "DeviceUserId" });
            DropIndex("dbo.Orders", new[] { "ClientId" });
            DropIndex("dbo.Carts", new[] { "DeviceUserId" });
            DropIndex("dbo.CartProductItems", new[] { "OrderId" });
            DropIndex("dbo.CartProductItems", new[] { "CartId" });
            DropIndex("dbo.CartProductItems", new[] { "CartProductVariantId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Shops");
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LogEntries");
            DropTable("dbo.Fees");
            DropTable("dbo.Devices");
            DropTable("dbo.ClientTransactions");
            DropTable("dbo.ClientDiscounts");
            DropTable("dbo.Transfers");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.Payments");
            DropTable("dbo.Checks");
            DropTable("dbo.Cashes");
            DropTable("dbo.Sizes");
            DropTable("dbo.Products");
            DropTable("dbo.ProductVariants");
            DropTable("dbo.Colors");
            DropTable("dbo.Categories");
            DropTable("dbo.CartProductVariants");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Documents");
            DropTable("dbo.Clients");
            DropTable("dbo.Orders");
            DropTable("dbo.DeviceUsers");
            DropTable("dbo.Carts");
            DropTable("dbo.CartProductItems");
            DropTable("dbo.Brands");
            DropTable("dbo.Banners");
            DropTable("dbo.Banks");
        }
    }
}
