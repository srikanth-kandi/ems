using System.Net;
using System.Text.Json;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests;

public class HealthCheckTests : TestBase
{
    public HealthCheckTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await Client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.GetProperty("status").GetString().Should().Be("Healthy");
        result.GetProperty("timestamp").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}
