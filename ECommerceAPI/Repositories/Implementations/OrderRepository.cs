using ECommerceAPI.Data;
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

            // Calculate Total Amount
            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.Price);
            order.CreatedAt = DateTime.UtcNow;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
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
        }
    }
}
