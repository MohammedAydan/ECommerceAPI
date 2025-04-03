using System.Text.Json.Serialization;

namespace ECommerceAPI.Model.Entities
{
    public class FawaterakWebhookRequest
    {
        [JsonPropertyName("hashKey")]
        public string hashKey { get; set; }
        [JsonPropertyName("invoice_key")]
        public string invoice_key { get; set; }
        [JsonPropertyName("invoice_id")]
        public int invoice_id { get; set; }
        [JsonPropertyName("payment_method")]
        public string payment_method { get; set; }
        [JsonPropertyName("invoice_status")]
        public string invoice_status { get; set; }
        [JsonPropertyName("pay_load")]
        public object? pay_load { get; set; } 
        public string referenceNumber { get; set; }
    }



}
