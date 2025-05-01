// ... all using directives ...

using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Params;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, ICartService cartService, IMapper mapper)
        {
            _orderService = orderService;
            _cartService = cartService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            var orders = await _orderService.GetAllOrdersAsync(page, limit, search, sortBy, ascending, filters);
            return Ok(orders);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersByUser([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId, page, limit);
            return Ok(orders);
        }

        [HttpGet("user/with-items")]
        public async Task<IActionResult> GetOrdersWithItems([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] int pageItems = 1, [FromQuery] int limitItems = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var orders = await _orderService.GetOrdersWithItemsAsync(userId, page, limit, true, pageItems, limitItems);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(string id, [FromQuery] bool includeProducts = false, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var order = await _orderService.GetOrderByIdAsync(id, includeProducts, page, limit);
            if (order == null)
                return NotFound("Order not found.");

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutParams checkoutParams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized.");

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            var orderDTO = new OrderDTO
            {
                UserId = userId,
                PaymentMethod = checkoutParams.paymentMethod,
                ShippingAddress = checkoutParams.ShippingAddress,
                ShippingPrice = checkoutParams.ShippingPrice,
                OrderItems = _mapper.Map<IEnumerable<OrderItemDTO>>(cart.CartItems).ToList()
            };

            var result = await _orderService.CreateOrderAsync(orderDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderDTO orderDTO)
        {
            if (id != orderDTO.Id)
                return BadRequest("Order ID mismatch.");

            await _orderService.UpdateOrderAsync(orderDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
