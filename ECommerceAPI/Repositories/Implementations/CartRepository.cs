using ECommerceAPI.Data;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddToCartAsync(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _context.Carts.AddAsync(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                };
                cart.CartItems.Add(cartItem);
                await IncrementProductCartCountAsync(productId);
            }

            await _context.SaveChangesAsync();
        }

        private async Task IncrementProductCartCountAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.CartAddedCount += 1;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(string userId, int productId, bool removeAll = false)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null) return;

            if (removeAll || item.Quantity <= 1)
            {
                cart.CartItems.Remove(item);
            }
            else
            {
                item.Quantity -= 1;
            }

            await DecrementProductCartCountAsync(productId);
            await _context.SaveChangesAsync();
        }

        private async Task DecrementProductCartCountAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null && product.CartAddedCount > 0)
            {
                product.CartAddedCount -= 1;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
