using ECommerceAPI.Data;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(
            int? page = 1,
            int? limit = 10,
            string? search = null,
            string? sortBy = "CreatedAt",
            bool ascending = false,
            Dictionary<string, string>? filters = null)
        {
            page = Math.Max(1, page.Value);
            limit = Math.Max(1, limit.Value);

            IQueryable<Order> query = _context.Orders.AsQueryable();

            // Search (applies to string fields — you can customize this)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.Id.Contains(search)); // Adjust "OrderNumber" to match your schema
            }

            // Apply filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var propertyInfo = typeof(Order).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) continue;

                    var parameter = Expression.Parameter(typeof(Order), "x");
                    var property = Expression.Property(parameter, propertyInfo);

                    object? typedValue;
                    try
                    {
                        typedValue = Convert.ChangeType(filter.Value, propertyInfo.PropertyType);
                    }
                    catch
                    {
                        continue; // Skip this filter if conversion fails
                    }

                    var constant = Expression.Constant(typedValue);
                    Expression condition;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        condition = Expression.Call(property, containsMethod!, constant);
                    }
                    else
                    {
                        condition = Expression.Equal(property, constant);
                    }

                    var lambda = Expression.Lambda<Func<Order, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var propertyInfo = typeof(Order).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var parameter = Expression.Parameter(typeof(Order), "x");
                    var property = Expression.Property(parameter, propertyInfo);
                    var sortLambda = Expression.Lambda(property, parameter);

                    string methodName = ascending ? "OrderBy" : "OrderByDescending";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(Order), propertyInfo.PropertyType);

                    query = (IQueryable<Order>)method.Invoke(null, new object[] { query, sortLambda })!;
                }
            }

            // Apply pagination
            query = query.Skip((page.Value - 1) * limit.Value).Take(limit.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            page = Math.Max(1, page);
            limit = Math.Max(1, limit);

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(string userId, int page = 1, int limit = 10, bool includeProducts = false, int pageItems = 1, int limitItems = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            page = Math.Max(1, page);
            limit = Math.Max(1, limit);
            pageItems = Math.Max(1, pageItems);
            limitItems = Math.Max(1, limitItems);

            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .AsQueryable();

            if (includeProducts)
            {
                query = query.Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.Product);
            }

            return await query.ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(string id, bool includeProducts = false, int page = 1, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(id));

            var query = _context.Orders.AsQueryable();

            if (includeProducts)
            {
                query = query.Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.Product);
            }

            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetOrderByInvoiceIdAsync(string invoiceId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                throw new ArgumentException("Invoice ID cannot be null or empty.", nameof(invoiceId));

            return await _context.Orders
                .FirstOrDefaultAsync(o => o.InvoiceId == invoiceId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new ArgumentException("Order must contain at least one item.", nameof(order));

            var productIds = order.OrderItems.Select(item => item.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            foreach (var item in order.OrderItems)
            {
                item.Id ??= Guid.NewGuid().ToString();

                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");

                item.Product = product;
                item.Price = calcPrice(1, product.Price , product.discount);
                //item.Price = product.discount != null?product.Price - (product.Price * ((decimal)product.discount / 100)): product.Price;
            }

            order.TotalAmount = (order.OrderItems.Sum(item => calcPrice(item.Quantity, item.Product.Price, item.Product.discount)) + (order.ShippingPrice ?? 0));
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";

            await _context.Orders.AddAsync(order);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();

                foreach (var item in order.OrderItems)
                {
                    await IncrementProductCartCountAsync(item.ProductId);
                }

                await transaction.CommitAsync();
                var user = await _context.Users.FindAsync(order.UserId);
                order.User = user;
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(id));

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            foreach (var item in order.OrderItems)
            {
                await DecrementProductCartCountAsync(item.ProductId);
            }
        }

        private async Task IncrementProductCartCountAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.CartAddedCount = (product.CartAddedCount ?? 0) + 1;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
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

        private decimal calcPrice(int quantity, decimal price, int? discount)
        {
            if (discount != null && discount > 0)
            {
                return quantity * (price - (price * ((decimal)discount / 100)));
            }
            else
            {
                return quantity * price;
            }
        }
    }
}
