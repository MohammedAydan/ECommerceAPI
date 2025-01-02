using ECommerceAPI.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task AddOrderAsync(OrderDTO orderDTO);
        Task UpdateOrderAsync(OrderDTO orderDTO);
        Task DeleteOrderAsync(int id);
    }
}