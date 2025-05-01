namespace ECommerceAPI.Model.Params
{
    public class CheckoutParams
    {
        public string paymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
