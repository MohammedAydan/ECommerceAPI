using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders([FromQuery] int? page = 1,[FromQuery] int? limit = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync(page, limit);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id,
            [FromQuery] bool getMyItemsAndProducts = false, 
            [FromQuery] int? page = 1, 
            [FromQuery] int? limit = 10)
        {
            var order = await _orderService.GetOrderByIdAsync(id, getMyItemsAndProducts, page, limit);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> AddOrder(OrderDTO orderDTO)
        {
            await _orderService.AddOrderAsync(orderDTO);
            return CreatedAtAction(nameof(GetOrder), new { id = orderDTO.OrderId }, orderDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return BadRequest();
            }

            await _orderService.UpdateOrderAsync(orderDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}