using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(int page = 1, int limit = 10);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10); 
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync(
            string userId, 
            int page = 1, 
            int limit = 10,
            bool getMyItemsAndProducts = false, 
            int pageItems = 1, 
            int limitItems = 10
            );
        Task<Order> GetOrderByIdAsync(string id, bool getMyItemsAndProducts = false, int page = 1, int limit = 10);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(string id);

        Task<Order> GetOrderByInvoiceIdAsync(string id);
    }
}