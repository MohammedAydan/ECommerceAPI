using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int page = 1, int limit = 10);
        Task<IEnumerable<OrderDTO>> GetAllOrdersByUserIdAsync(string userId, int page = 1, int limit = 10);
        Task<IEnumerable<OrderDTO>> GetAllOrdersByUserIdAsync(
            string userId,
            int page = 1,
            int limit = 10,
            bool getMyItemsAndProducts = false,
            int pageItems = 1,
            int limitItems = 10
            );
        Task<OrderDTO> GetOrderByIdAsync(string id, bool getMyItemsAndProducts = false, int page = 1, int limit = 10);
        Task<Order> CreateOrderAsync(OrderDTO orderDTO);
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderAsync(OrderDTO orderDto);
        Task DeleteOrderAsync(string id);

        Task<Order> GetOrderByInvoiceIdAsync(string id);
    }
}