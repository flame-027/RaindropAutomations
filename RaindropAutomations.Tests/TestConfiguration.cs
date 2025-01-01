using Microsoft.Extensions.Configuration;

namespace RaindropAutomations.Tests
{
    public static class TestConfiguration
    {
        public static IConfiguration BuildTestConfiguration()
        {
            var configData = new Dictionary<string, string>
            {
                { "Raindrop:ApiToken", "test-token" },
                { "Raindrop:ApiBaseUrl", "https://fake-api.com" }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }
    }
}
