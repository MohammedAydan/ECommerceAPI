using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Data;

namespace ECommerceAPI.Helpers
{
    public class SeedHelper
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<RoleModel> _roleManager;
        private readonly AppDbContext _context;

        public SeedHelper(UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedAsync()
        {
            if((await _context.Database.GetPendingMigrationsAsync()).Any()) {
                await _context.Database.MigrateAsync();
            }

            if (!_context.Roles.Any())
            {
                var roles = new[] { "Admin", "User", "Delivery" };
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new RoleModel { Name = role });
                    }
                }
            }

            if (!_context.Users.Any())
            {
                var user = new UserModel
                {
                    UserName = "Admin",
                    Email = "admin@admin.com",
                    Address = "null",
                    City = "null",
                    Country = "null",
                };

                var result = await _userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
