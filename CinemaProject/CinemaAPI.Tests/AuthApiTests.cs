using System.Net;
using System.Net.Http.Json;
using CinemaAPI.Models.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace CinemaAPI.Tests;

public class AuthApiTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly ITestOutputHelper _output;

    public AuthApiTests(IntegrationTestFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserProfile()
    {
        // Arrange
        _output.WriteLine("=== ARRANGE: Creating authorized client with 'admin' role ===");
        var client = _factory.CreateAuthorizedClient("admin");

        // Act
        _output.WriteLine("=== ACT: Sending GET request to /api/Auth/me ===");
        var response = await client.GetAsync("/api/Auth/me");

        // Assert
        _output.WriteLine($"=== ASSERT: Response Status Code = {response.StatusCode} ===");

        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"=== Response Body: {content} ===");

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        _output.WriteLine($"=== User: {user.Name} {user.Surname} ({user.Email}) ===");

        Assert.NotNull(user);
        Assert.Equal("admin@cinema.com", user.Email);
        Assert.Equal("Admin", user.Name);
        Assert.Equal("Adminov", user.Surname);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        _output.WriteLine("=== ARRANGE: Creating client WITHOUT token ===");
        var client = _factory.CreateClient();

        // Act
        _output.WriteLine("=== ACT: Sending GET request to /api/Auth/me (no auth) ===");
        var response = await client.GetAsync("/api/Auth/me");

        // Assert
        _output.WriteLine($"=== ASSERT: Response Status Code = {response.StatusCode} ===");
        _output.WriteLine("=== Expected: Unauthorized (401) ===");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidToken_UpdatesProfile()
    {
        // Arrange
        _output.WriteLine("=== ARRANGE: Creating authorized client with 'client' role ===");
        var client = _factory.CreateAuthorizedClient("client");

        var updateDto = new UserResponseDto
        {
            Name = "John Updated",
            Surname = "Doe Updated",
            Email = "john.updated@example.com",
            Gender = "Male",
            Description = "Updated description"
        };
        _output.WriteLine($"=== Update Data: {updateDto.Name} {updateDto.Surname} ===");

        // Act
        _output.WriteLine("=== ACT: Sending PUT request to /api/Auth/me ===");
        var response = await client.PutAsJsonAsync("/api/Auth/me", updateDto);

        // Assert
        _output.WriteLine($"=== ASSERT: Response Status Code = {response.StatusCode} ===");

        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"=== Response Body: {content} ===");

        response.EnsureSuccessStatusCode();
    }
}
