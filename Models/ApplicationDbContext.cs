using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Models.BrandModels;
using _3.QKA_DACK.Models.CartModels;
using _3.QKA_DACK.Models.CategoryModels;
using _3.QKA_DACK.Models.OrderModels;
using _3.QKA_DACK.Models.ProductModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _3.QKA_DACK.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Review> Reviews { get; set; }
    }
}
