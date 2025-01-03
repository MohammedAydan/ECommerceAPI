using ECommerceAPI.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int? page = 1, int? limit = 10);
        Task<OrderDTO> GetOrderByIdAsync(int id, bool getMyItemsAndProducts = false, int? page = 1, int? limit = 10);
        Task AddOrderAsync(OrderDTO orderDTO);
        Task UpdateOrderAsync(OrderDTO orderDTO);
        Task DeleteOrderAsync(int id);
    }
}