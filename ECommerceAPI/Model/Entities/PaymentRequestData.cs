namespace ECommerceAPI.Model.Entities
{
    public class PaymentRequestData
    {
        public string PaymentMethodId { get; set; }
        public string CartTotal { get; set; }
        public string Currency { get; set; }
        public Customer Customer { get; set; }
        public RedirectionUrls RedirectionUrls { get; set; }
        public List<PaymentCartItem> CartItems { get; set; }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class RedirectionUrls
    {
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public string PendingUrl { get; set; }
    }

    public class PaymentCartItem
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }

}
