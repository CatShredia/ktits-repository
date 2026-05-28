using System.Net;
using System.Net.Http.Json;
using ProductionSystem.Api.Tests.Infrastructure;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Tests.Api;

public class AuthApiTests : ApiTestBase
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        using var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            login = TestDataSeeder.CustomerLogin,
            password = TestDataSeeder.CustomerPassword,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(TestDataSeeder.CustomerLogin, body!.Login);
        Assert.Equal(UserRoles.Customer, body.Role);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        using var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            login = TestDataSeeder.CustomerLogin,
            password = "Wrong1",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_ValidCustomer_ReturnsOkWithToken()
    {
        using var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            login = "new_customer_1",
            password = "Valid1",
            fullName = "Новый заказчик",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(UserRoles.Customer, body!.Role);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        using var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            login = "weak_pwd_user",
            password = "weak",
            fullName = "Test",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateLogin_ReturnsConflict()
    {
        using var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            login = TestDataSeeder.CustomerLogin,
            password = "Valid1",
            fullName = "Дубликат",
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
