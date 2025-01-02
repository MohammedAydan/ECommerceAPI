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

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
