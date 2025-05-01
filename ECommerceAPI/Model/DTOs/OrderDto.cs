﻿using ECommerceAPI.Helpers;

namespace ECommerceAPI.Model.DTOs
{
    public class OrderDTO
    {
        public string Id { get; set; } = NumberGeneratorHelper.GenerateNumber();
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public string PaymentMethod { get; set; } = "CashOnDelivery";
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal? ShippingPrice { get; set; } = 0;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}