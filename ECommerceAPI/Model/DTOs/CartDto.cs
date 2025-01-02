namespace ECommerceAPI.Model.DTOs
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public string UserId { get; set; } // Assuming you're using Identity for users
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }
}