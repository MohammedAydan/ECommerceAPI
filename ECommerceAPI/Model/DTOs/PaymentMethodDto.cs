namespace ECommerceAPI.Model.DTOs
{
    public class PaymentMethodDto
    {
        public string PaymentId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool Redirect { get; set; }
        public string Logo { get; set; }
    }
}
