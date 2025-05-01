using ECommerceAPI.Data;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            // Fetch total counts
            var totalOrders = await _context.Orders.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var activeUsers = await _context.Users.CountAsync(u => u.LastSignIn >= thirtyDaysAgo);

            // Calculate total revenue
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);

            // OPTIONAL: Example for revenue growth (assuming CreatedAt date field exists in Orders)
            var now = DateTime.UtcNow;
            var lastMonth = now.AddMonths(-1);
            var twoMonthsAgo = now.AddMonths(-2);

            var revenueThisMonth = await _context.Orders
                .Where(o => o.CreatedAt >= lastMonth)
                .SumAsync(o => o.TotalAmount);

            var revenueLastMonth = await _context.Orders
                .Where(o => o.CreatedAt >= twoMonthsAgo && o.CreatedAt < lastMonth)
                .SumAsync(o => o.TotalAmount);

            var revenueGrowth = revenueLastMonth == 0
                ? 100
                : ((revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100;

            // Orders growth
            var ordersThisMonth = await _context.Orders.CountAsync(o => o.CreatedAt >= lastMonth);
            var ordersLastMonth = await _context.Orders.CountAsync(o => o.CreatedAt >= twoMonthsAgo && o.CreatedAt < lastMonth);
            var ordersGrowth = ordersLastMonth == 0 ? ordersThisMonth : ordersThisMonth - ordersLastMonth;

            // Product growth (based on creation date)
            var productsThisMonth = await _context.Products.CountAsync(p => p.CreatedAt >= lastMonth);
            var productsLastMonth = await _context.Products.CountAsync(p => p.CreatedAt >= twoMonthsAgo && p.CreatedAt < lastMonth);
            var productsGrowth = productsLastMonth == 0 ? productsThisMonth : productsThisMonth - productsLastMonth;

            // Users growth
            var usersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= lastMonth);
            var usersLastMonth = await _context.Users.CountAsync(u => u.CreatedAt >= twoMonthsAgo && u.CreatedAt < lastMonth);
            var usersGrowth = usersLastMonth == 0 ? usersThisMonth : usersThisMonth - usersLastMonth;

            return new DashboardStats
            {
                TotalRevenue = Math.Round(totalRevenue, 2),
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                ActiveUsers = activeUsers,
                RevenueGrowth = Math.Round(revenueGrowth, 2),
                OrdersGrowth = ordersGrowth,
                ProductsGrowth = productsGrowth,
                UsersGrowth = usersGrowth
            };
        }
    }
}
