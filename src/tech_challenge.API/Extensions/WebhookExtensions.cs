namespace tech_challenge.API.Extensions
{
    public static class WebhookExtensions
    {
        public static bool IsValidWebhook(
            this HttpRequest request,
            IConfiguration configuration)
        {
            var configuredKey = configuration["Webhook:ApiKey"];

            return request.Headers.TryGetValue("X-Webhook-Key", out var apiKey)
                   && apiKey == configuredKey;
        }
    }
}
