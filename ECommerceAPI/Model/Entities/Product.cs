using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Model.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [StringLength(100)]
        public string SKU { get; set; }

        public int StockQuantity { get; set; } = 0;
        [NotMapped]
        public virtual IFormFile? Image { get; set; }
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public int? discount { get; set; } = 0;
        public int? rating { get; set; } = 1;
        public int? CartAddedCount { get; set; } = 0;
        public int? CreatedOrderCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}