using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(page, limit);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving orders: {ex.Message}");
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllOrdersByUserIdAsync([FromQuery] int page = 1,[FromQuery] int limit = 10)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated or not authorized.");
                }

                var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, page, limit);
                if (orders == null || orders.IsNullOrEmpty())
                {
                    return NotFound("No orders found for the user.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving user orders: {ex.Message}");
            }
        }

        [HttpGet("user/getItems")]
        public async Task<IActionResult> GetAllOrdersByUserIdAsync([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] int pageItems = 1, [FromQuery] int limitItems = 10)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated or not authorized.");
                }

                var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, page, limit, true, pageItems, limitItems);
                if (orders == null || orders.IsNullOrEmpty())
                {
                    return NotFound("No orders found for the user.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving user orders: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(
            string id,
            [FromQuery] bool getMyItemsAndProducts = false,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id, getMyItemsAndProducts, page, limit);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the order: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] string paymentMethod)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(userId == null)
                {
                    return Unauthorized("User not authorized");
                }

                OrderDTO orderDTO = new OrderDTO();
                orderDTO.UserId = userId;
                orderDTO.PaymentMethod = paymentMethod;

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                orderDTO.OrderItems = _mapper.Map<IEnumerable<OrderItemDTO>>(cart.CartItems).ToList();

                var res = await _orderService.CreateOrderAsync(orderDTO);


                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the order: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, OrderDTO orderDTO)
        {
            try
            {
                if (id != orderDTO.Id)
                {
                    return BadRequest("Order ID mismatch.");
                }

                await _orderService.UpdateOrderAsync(orderDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the order: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }
    }
}
