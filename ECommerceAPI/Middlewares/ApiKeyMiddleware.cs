namespace ECommerceAPI.Middlewares
{
    public class ApiKeyMiddleware : IMiddleware
    {
        private readonly string _apiKey;

        public ApiKeyMiddleware(IConfiguration configuration)
        {
            _apiKey = configuration["ApiSettings:ApiKey"] ?? throw new ArgumentNullException("ApiSettings:ApiKey", "API Key is missing from configuration");
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing!");
                return;
            }

            if (!_apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key!");
                return;
            }

            await next(context);
        }
    }
}
