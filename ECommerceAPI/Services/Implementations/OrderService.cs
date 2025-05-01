using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int? page = 1, int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(page, limit, search, sortBy, ascending, filters);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, page, limit);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersWithItemsAsync(string userId, int page = 1, int limit = 10, bool includeProducts = false, int pageItems = 1, int limitItems = 10)
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync(userId, page, limit, includeProducts, pageItems, limitItems);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(string id, bool includeProducts = false, int page = 1, int limit = 10)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id, includeProducts, page, limit);
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<Order> GetOrderByInvoiceIdAsync(string invoiceId)
        {
            return await _orderRepository.GetOrderByInvoiceIdAsync(invoiceId);
        }

        public async Task<Order> CreateOrderAsync(OrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task UpdateOrderAsync(OrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }
    }
}
