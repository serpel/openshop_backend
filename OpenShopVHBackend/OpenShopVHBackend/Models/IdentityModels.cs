using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OpenShopVHBackend.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string ProfileUrl { get; set; }
        public string Culture { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public ApplicationDbContext(string nameOrConnectionString)
           : base(nameOrConnectionString, throwIfV1Schema: false)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductVariant> ProductVariants { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<Shop> Shops { get; set; }

        public DbSet<Banner> Banners { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<DeviceUser> DeviceUser { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartProductItem> CartProductItems { get; set; }

        public DbSet<CartProductVariant> CartProductVariants { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Document> Documents { get; set; }
  
        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<ClientDiscount> ClientDiscounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cash> Cash { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<InvoiceItem> Invoices { get; set; }
        public DbSet<InvoiceHistory> InvoiceHistory { get; set; }
        public DbSet<Fees> Fees { get; set; }
        public DbSet<ClientTransactions> ClientTransactions { get; set; }
        public DbSet<WishlistItem> WishlistItem { get; set; }
        public DbSet<WishlistProductVariant> WishlistProductVariant { get; set; }
    }
}