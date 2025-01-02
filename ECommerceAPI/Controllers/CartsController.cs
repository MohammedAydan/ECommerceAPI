using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDTO>> GetCartByUserId(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<CartDTO>> AddCart(CartDTO cartDTO)
        {
            await _cartService.AddCartAsync(cartDTO);
            return CreatedAtAction(nameof(GetCartByUserId), new { userId = cartDTO.UserId }, cartDTO);
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, CartDTO cartDTO)
        {
            if (cartId != cartDTO.CartId)
            {
                return BadRequest();
            }

            await _cartService.UpdateCartAsync(cartDTO);
            return NoContent();
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            await _cartService.DeleteCartAsync(cartId);
            return NoContent();
        }
    }
}