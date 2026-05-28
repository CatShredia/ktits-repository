using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Tests.Infrastructure;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TYFp3oI6yY71aNuyrkjJBFzi5pVaaT82",
                ["Jwt:Issuer"] = "ProductionSystem",
                ["Jwt:Audience"] = "ProductionSystem.Client",
                ["InMemoryDatabaseName"] = _dbName,
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using (var scope = Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await TestDataSeeder.ClearAsync(db);
        }

        await using var seedScope = Services.CreateAsyncScope();
        var seedDb = seedScope.ServiceProvider.GetRequiredService<AppDbContext>();
        await TestDataSeeder.SeedAsync(seedDb);
    }

    public HttpClient CreateAuthenticatedClient(string login, string password)
    {
        var client = CreateClient();
        var token = LoginAsync(client, login, password).GetAwaiter().GetResult();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<string> LoginAsync(HttpClient client, string login, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { login, password });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        return body.Token;
    }
}

internal sealed class AuthResponseDto
{
    public string Token { get; set; } = "";
    public string Login { get; set; } = "";
    public string Role { get; set; } = "";
}
