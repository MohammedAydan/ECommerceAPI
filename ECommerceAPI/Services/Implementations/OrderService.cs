﻿using AutoMapper;
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

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync(int? page = 1, int? limit = 10)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(page, limit);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id, bool getMyItemsAndProducts = false, int? page = 1, int? limit = 10)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id, getMyItemsAndProducts, page, limit);
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task AddOrderAsync(OrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            await _orderRepository.AddOrderAsync(order);
        }

        public async Task UpdateOrderAsync(OrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }
    }
}