using AutoMapper;
using ECommerceAPI.EmailTemplates;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
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
        private readonly IEmailService _emailService;

        public CheckoutController(
            IOrderService orderService,
            ICartService cartService,
            IMapper mapper,
            IEmailService emailService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDTO request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User authentication failed." });
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart?.CartItems == null || !cart.CartItems.Any())
                {
                    return BadRequest(new { Message = "Your cart is empty." });
                }

                var orderDto = new OrderDTO
                {
                    UserId = userId,
                    PaymentMethod = request.PaymentMethod ?? "CashOnDelivery",
                    ShippingAddress = request.ShippingAddress,
                    ShippingPrice = request.ShippingPrice,
                    OrderItems = _mapper.Map<List<OrderItemDTO>>(cart.CartItems),
                    Status = "Pending"
                };

                var order = await _orderService.CreateOrderAsync(orderDto);
                if (order == null)
                {
                    return StatusCode(500, new { Message = "Order creation failed." });
                }

                // Clear cart after successful order creation
                if (cart.CartId != null)
                {
                    await _cartService.DeleteCartAsync(cart.CartId.Value);
                }

                // Send confirmation email
                OrderConfirmationTemplate orderConfirmationTemplate = new OrderConfirmationTemplate(
                    order.User?.UserName ?? "",
                    order.Id.ToString(),
                    order.CreatedAt.ToString(),
                    order.ShippingAddress ?? "",
                    order.TotalAmount.ToString("F2"),
                    order.PaymentMethod,
                    GetOrderUrl(order.Id.ToString())
                    );

                var emailSubject = orderConfirmationTemplate.Subject;
                var emailTo = order.User?.Email;

                if (!string.IsNullOrEmpty(emailTo))
                {
                    await _emailService.SendEmail(
                        new EmailMessage(
                            new[] { emailTo },
                            emailSubject,
                            orderConfirmationTemplate.Body
                            )
                        );
                }

                return Ok(new
                {
                    Code = 200,
                    Message = "Order created successfully",
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Checkout process failed",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        private string GetOrderUrl(string orderId)
        {
            return $"https://ecommerce-app-rho-peach.vercel.app/orders/{orderId}";
        }
    }

    public class CheckoutRequestDTO
    {
        public string? PaymentMethod { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal ShippingPrice { get; set; }
    }
}