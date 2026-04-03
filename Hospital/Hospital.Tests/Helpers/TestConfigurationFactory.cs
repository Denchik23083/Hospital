using Microsoft.Extensions.Configuration;

namespace Hospital.Tests.Helpers
{
    public static class TestConfigurationFactory
    {
        public static IConfiguration Create()
        {
            var settings = new Dictionary<string, string?>
            {
                ["SecretKey"] = "super_secret_key_123456789_super_secret",
                ["Issuer"] = "test-issuer",
                ["Audience"] = "test-audience"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }
    }
}