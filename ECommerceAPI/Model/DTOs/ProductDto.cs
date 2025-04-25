using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Model.DTOs
{
    public class ProductDTO
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int? discount { get; set; } = 0;
        public int? rating { get; set; } = 1;
        [BindNever]
        [NotMapped]
        public decimal salePrice
        { 
            get
            {
                if(discount > 0) { 
                    return Price - (Price * ((decimal)discount / 100));
                }
                else
                {
                    return Price;
                }
            }
        }
        [BindNever]
        public int? CartAddedCount { get; set; } = 0;
        [BindNever]
        public int? CreatedOrderCount { get; set; } = 0;
        [BindNever]
        public virtual CategoryDTO? Category { get; set; } = null;
    }
}