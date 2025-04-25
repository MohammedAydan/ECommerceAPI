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

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int page = 1, int limit = 10)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(page, limit);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersByUserIdAsync(string userId, int page = 1, int limit = 10) 
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId,page,limit);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersByUserIdAsync(
            string userId,
            int page = 1,
            int limit = 10,
            bool getMyItemsAndProducts = false,
            int pageItems = 1,
            int limitItems = 10
            )
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync(userId, page, limit, getMyItemsAndProducts, pageItems, limitItems);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(string id, bool getMyItemsAndProducts = false, int page = 1, int limit = 10)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id, getMyItemsAndProducts, page, limit);
            return _mapper.Map<OrderDTO>(order, opt => opt.AfterMap((src, dest) =>
            {
                // Custom logic for mapping OrderItems
                dest.OrderItems = _mapper.Map<List<OrderItemDTO>>(dest.OrderItems);
            }));
        }


        public async Task<Order> GetOrderByInvoiceIdAsync(string id)
        {
            return await _orderRepository.GetOrderByInvoiceIdAsync(id);
        }

        public async Task<Order> CreateOrderAsync(OrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task UpdateOrderAsync(OrderDTO orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }
    }
}