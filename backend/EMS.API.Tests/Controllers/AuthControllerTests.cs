using System.Net;
using System.Text;
using System.Text.Json;
using EMS.API.DTOs;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class AuthControllerTests : TestBase
{
    public AuthControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "password123"
        };
        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        authResponse.Should().NotBeNull();
        authResponse.Token.Should().NotBeNullOrEmpty();
        authResponse.Username.Should().Be("admin");
        authResponse.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "wrongpassword"
        };
        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "", // Invalid: empty username
            Password = "password123"
        };
        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "password123",
            Role = "User"
        };
        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        authResponse.Should().NotBeNull();
        authResponse.Token.Should().NotBeNullOrEmpty();
        authResponse.Username.Should().Be("newuser");
        authResponse.Role.Should().Be("User");
    }

    [Fact]
    public async Task Register_WithExistingUsername_ReturnsConflict()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "admin", // Already exists
            Email = "newemail@test.com",
            Password = "password123",
            Role = "User"
        };
        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "invalid-email", // Invalid email format
            Password = "password123",
            Role = "User"
        };
        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/auth/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
