using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Model.Entities
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [Required]
        public string UserId { get; set; } // Assuming you're using Identity for users

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}