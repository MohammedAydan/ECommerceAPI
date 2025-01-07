namespace ECommerceAPI.Model.DTOs
{
    public class OrderDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; } // Assuming you're using Identity for users
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO>? OrderItems { get; set; } = new List<OrderItemDTO>();
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}