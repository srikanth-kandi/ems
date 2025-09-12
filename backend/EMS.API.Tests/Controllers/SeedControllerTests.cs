using System.Net;
using System.Text.Json;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class SeedControllerTests : TestBase
{
    public SeedControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SeedData_ReturnsSuccess()
    {
        // Act
        var response = await Client.PostAsync("/api/seed/seed", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.GetProperty("message").GetString().Should().Be("Database seeded successfully");
    }

    [Fact]
    public async Task ReseedData_ReturnsSuccess()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.PostAsync("/api/seed/reseed", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.GetProperty("message").GetString().Should().Be("Database reseeded successfully");
    }

    [Fact]
    public async Task ClearData_ReturnsSuccess()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.DeleteAsync("/api/seed/clear");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.GetProperty("message").GetString().Should().Be("Database cleared successfully");
    }

    [Fact]
    public async Task GetSeedStatus_ReturnsStatus()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/seed/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.GetProperty("departments").GetInt32().Should().Be(3);
        result.GetProperty("employees").GetInt32().Should().Be(3);
        result.GetProperty("attendances").GetInt32().Should().Be(2);
        result.GetProperty("users").GetInt32().Should().Be(2);
    }
}
