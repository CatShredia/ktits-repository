using System.Net;
using System.Net.Http.Json;
using CinemaAPI.Models.DTOs;
using Xunit;

namespace CinemaAPI.Tests;

/// <summary>
/// Integration tests for Authentication API endpoints (register, login, user profile)
/// </summary>
public class AuthApiTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public AuthApiTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_NewUser_CreatesUserSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new UserRegisterDto
        {
            Name = "Alice",
            Surname = "Smith",
            Email = "alice@example.com",
            Login = "alicesmith",
            Password = "securepassword123",
            Gender = "Female",
            Description = "New user"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal("alice@example.com", result.User.Email);
        Assert.Equal("Alice", result.User.Name);
        Assert.Equal("Smith", result.User.Surname);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new UserRegisterDto
        {
            Name = "Duplicate",
            Surname = "User",
            Email = "admin@cinema.com", // Already exists in seed data
            Login = "duplicateuser",
            Password = "password123",
            Gender = "Male",
            Description = "Duplicate email test"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "admin" // Password matches seed data hash
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal("admin@cinema.com", result.User.Email);
        Assert.Equal("Admin", result.User.Name);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "wrongpassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new UserLoginDto
        {
            Login = "nonexistent",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserProfile()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");

        // Act
        var response = await client.GetAsync("/api/Auth/me");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.NotNull(user);
        Assert.Equal("admin@cinema.com", user.Email);
        Assert.Equal("Admin", user.Name);
        Assert.Equal("Adminov", user.Surname);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidToken_UpdatesProfile()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("client");
        var updateDto = new UserResponseDto
        {
            Name = "John Updated",
            Surname = "Doe Updated",
            Email = "john.updated@example.com",
            Gender = "Male",
            Description = "Updated description"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/Auth/me", updateDto);

        // Assert - API returns success status
        response.EnsureSuccessStatusCode();
    }
}
