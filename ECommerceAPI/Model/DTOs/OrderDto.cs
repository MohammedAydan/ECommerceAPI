namespace ECommerceAPI.Model.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } // Assuming you're using Identity for users
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
}