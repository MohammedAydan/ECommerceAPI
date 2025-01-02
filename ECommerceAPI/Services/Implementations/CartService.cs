using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task AddCartAsync(CartDTO cartDTO)
        {
            var cart = _mapper.Map<Cart>(cartDTO);
            await _cartRepository.AddCartAsync(cart);
        }

        public async Task UpdateCartAsync(CartDTO cartDTO)
        {
            var cart = _mapper.Map<Cart>(cartDTO);
            await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCartAsync(cartId);
        }
    }
}