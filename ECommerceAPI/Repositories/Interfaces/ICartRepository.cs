﻿using ECommerceAPI.Model.Entities;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(int cartId);
    }
}