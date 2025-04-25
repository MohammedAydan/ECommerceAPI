using ECommerceAPI.Data;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int page = 1, int limit = 10)
        {
            page = Math.Max(1, page);
            limit = Math.Max(1, limit);

            return await _context.Orders
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            page = Math.Max(1, page);
            limit = Math.Max(1, limit);

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(
            string userId,
            int page = 1,
            int limit = 10,
            bool includeItemsAndProducts = false,
            int itemsPage = 1,
            int itemsLimit = 10)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            page = Math.Max(1, page);
            limit = Math.Max(1, limit);
            itemsPage = Math.Max(1, itemsPage);
            itemsLimit = Math.Max(1, itemsLimit);

            var query = _context.Orders.Where(o => o.UserId == userId)
                .Skip((page - 1) * limit)
                .Take(limit);

            if (includeItemsAndProducts)
            {
                query = query.Include(o => o.OrderItems
                    .Skip((itemsPage - 1) * itemsLimit)
                    .Take(itemsLimit))
                    .ThenInclude(oi => oi.Product);
            }

            return await query.ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(string id, bool includeItemsAndProducts = false, int page = 1, int limit = 10)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(id));

            var query = _context.Orders.AsQueryable();

            if (includeItemsAndProducts)
            {
                page = Math.Max(1, page);
                limit = Math.Max(1, limit);

                query = query.Include(o => o.OrderItems
                        .Skip((page - 1) * limit)
                        .Take(limit))
                    .ThenInclude(oi => oi.Product);
            }

            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetOrderByInvoiceIdAsync(string id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.InvoiceId.Equals(id));
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new ArgumentException("Order must contain at least one item.", nameof(order));

            // Calculate total amount
            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.Price);
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";

            // Get distinct product IDs to avoid duplicate tracking
            var productIds = order.OrderItems
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // Pre-load all products in a single query
            var existingProducts = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            // Attach products to order items
            foreach (var item in order.OrderItems)
            {
                if (existingProducts.TryGetValue(item.ProductId, out var product))
                {
                    item.Product = product;
                }
                else
                {
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");
                }
            }

            await _context.Orders.AddAsync(order);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();

                // Update product cart counts - optimized batch update
                var updateTasks = order.OrderItems
                    .Select(item => IncrementProductCartCountAsync(item.ProductId))
                    .ToList();

                await Task.WhenAll(updateTasks);

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task IncrementProductCartCountAsync(int productId)
        {
           // // Using direct SQL for performance (avoid tracking)
           // await _context.Database.ExecuteSqlInterpolatedAsync(
           //     $@"UPDATE Products 
           //SET CartAddedCount = ISNULL(CartAddedCount, 0) + 1 
           //WHERE ProductId = {productId}");
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(id));

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            foreach (var item in order.OrderItems)
            {
                await DecrementProductCartCountAsync(item.ProductId);
            }
        }

        private async Task DecrementProductCartCountAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.CartAddedCount = (product.CartAddedCount ?? 0) - 1;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
