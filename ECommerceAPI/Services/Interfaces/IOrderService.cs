using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int? page = 1, int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10);
        Task<IEnumerable<OrderDTO>> GetOrdersWithItemsAsync(string userId, int page = 1, int limit = 10, bool includeProducts = false, int pageItems = 1, int limitItems = 10);
        Task<OrderDTO> GetOrderByIdAsync(string id, bool includeProducts = false, int page = 1, int limit = 10);
        Task<Order> GetOrderByInvoiceIdAsync(string invoiceId);

        Task<Order> CreateOrderAsync(OrderDTO orderDTO);
        Task UpdateOrderAsync(OrderDTO orderDTO);
        Task DeleteOrderAsync(string id);
    }
}
