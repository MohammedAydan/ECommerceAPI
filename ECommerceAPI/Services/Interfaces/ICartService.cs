﻿using ECommerceAPI.Model.DTOs;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(CartDTO cartDTO);
        Task UpdateCartAsync(CartDTO cartDTO);
        Task DeleteCartAsync(int cartId);


        Task AddToCartAsync(string userId, int productId);
        Task RemoveFromCartAsync(string userId, int productId, bool removeAll = false);
    }
}