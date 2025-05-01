﻿using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(int? page = 1, int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int page = 1, int limit = 10);
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync(string userId, int page = 1, int limit = 10, bool includeProducts = false, int pageItems = 1, int limitItems = 10);
        Task<Order> GetOrderByIdAsync(string id, bool includeProducts = false, int page = 1, int limit = 10);
        Task<Order> GetOrderByInvoiceIdAsync(string invoiceId);

        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(string id);
    }
}
