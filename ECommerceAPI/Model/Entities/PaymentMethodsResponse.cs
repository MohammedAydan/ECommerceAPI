using System.Text.Json.Serialization;

namespace ECommerceAPI.Model.Entities
{
    public partial class PaymentMethodsResponse
    {
         
        [JsonPropertyName("status")]
        public string Status { get; set; }

         
        [JsonPropertyName("vendorSettingsData")]
        public VendorSettingsData VendorSettingsData { get; set; }

         
        [JsonPropertyName("data")]
        public List<Datum> Data { get; set; }
    }

    public partial class Datum
    {
         
        [JsonPropertyName("paymentId")]
        public long? PaymentId { get; set; }

         
        [JsonPropertyName("name_en")]
        public string Name_En { get; set; }

         
        [JsonPropertyName("name_ar")]
        public string Name_Ar { get; set; }

         
        [JsonPropertyName("redirect")]
        public bool? Redirect { get; set; }

         
        [JsonPropertyName("logo")]
        public Uri Logo { get; set; }
    }

    public partial class VendorSettingsData
    {
        [JsonPropertyName("custome_iframe_title")]
        public object CustomeIframeTitle { get; set; }
    }
}
