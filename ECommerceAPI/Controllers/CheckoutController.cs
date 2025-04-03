using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CheckoutController(IOrderService orderService, ICartService cartService, IMapper mapper)
        {
            _orderService = orderService;
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    return BadRequest("Cart is empty or does not exist.");
                }

                var orderDTO = new OrderDTO
                {
                    UserId = userId,
                    PaymentMethod = "CashOnDelivery",
                    OrderItems = _mapper.Map<List<OrderItemDTO>>(cart.CartItems)
                };

                var order = await _orderService.CreateOrderAsync(orderDTO);
                if (order == null)
                {
                    return StatusCode(500, "Failed to create order.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during checkout.", error = ex.Message });
            }
        }
    }
}
