using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.Entities
{
    public class PaymentMethod
    {
        [Key]
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool Redirect { get; set; }
        public string Logo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
