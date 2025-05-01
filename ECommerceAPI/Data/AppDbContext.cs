using ECommerceAPI.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data
{

    public class AppDbContext:IdentityDbContext<UserModel,RoleModel,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // User Indexes
            // -------------------------
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.CreatedAt);

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.LastSignIn);

            // -------------------------
            // Product Indexes
            // -------------------------
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductName);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CreatedAt);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Price);

            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.ProductName, p.CategoryId });

            // -------------------------
            // Category Indexes
            // -------------------------
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName);

            // -------------------------
            // Order Indexes
            // -------------------------
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.TotalAmount);

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.CreatedAt });

            // -------------------------
            // OrderItem Indexes (if exists)
            // -------------------------
            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.ProductId);
        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
