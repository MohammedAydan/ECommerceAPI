using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Model.DTOs
{
    public class OrderItemDTO
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public ProductDTO Product { get; set; }
    }
}