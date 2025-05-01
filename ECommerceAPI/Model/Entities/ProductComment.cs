using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.Entities
{
    public class ProductComment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ProductId { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
        public decimal Rating { get; set; } = 1;
        [Required]
        public string Title { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
