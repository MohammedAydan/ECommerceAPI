namespace ECommerceAPI.Helpers
{
    public class NumberGeneratorHelper
    {
        private static readonly Random _random = new Random();

        public static string GenerateNumber()
        {
            // Get current timestamp (seconds since 1970) to make it unique
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Add a random 2-3 digit number to make sure it's unique if two orders are at the same second
            int randomSuffix = _random.Next(100, 999);

            // Combine
            string orderNumber = $"{timestamp}{randomSuffix}";

            // Ensure it’s 12 digits (cut if longer, pad if shorter)
            if (orderNumber.Length > 12)
                orderNumber = orderNumber.Substring(0, 12);
            else if (orderNumber.Length < 12)
                orderNumber = orderNumber.PadLeft(12, '0');

            return orderNumber;
        }
    }
}
