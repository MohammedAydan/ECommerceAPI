using ECommerceAPI.Data;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int? page = 1, int? limit = 10)
        {
            return await _context.Orders
                .Skip((page!.Value - 1) * (limit ?? 10))
                .Take(limit ?? 10)
                //.Include(o => o.OrderItems)
                //.ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, bool getMyItemsAndProducts = false, int? page = 1, int? limit = 10)
        {
            if(getMyItemsAndProducts == true) 
            {
                return await _context.Orders
                    .Include(
                        o => o.OrderItems.Skip((page!.Value - 1) * (limit ?? 10)).Take(limit ?? 10)
                    )
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
            } 
            else
            {
                return await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == id);
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}