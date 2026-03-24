using System.Net;
using System.Net.Http.Json;
using CinemaAPI.Models.DTOs;
using Xunit;

namespace CinemaAPI.Tests;

public class AuthApiTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public AuthApiTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserProfile()
    {
        var client = _factory.CreateAuthorizedClient("admin");

        var response = await client.GetAsync("/api/Auth/me");

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
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/Auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidToken_UpdatesProfile()
    {
        var client = _factory.CreateAuthorizedClient("client");
        var updateDto = new UserResponseDto
        {
            Name = "John Updated",
            Surname = "Doe Updated",
            Email = "john.updated@example.com",
            Gender = "Male",
            Description = "Updated description"
        };

        var response = await client.PutAsJsonAsync("/api/Auth/me", updateDto);

        response.EnsureSuccessStatusCode();
    }
}
