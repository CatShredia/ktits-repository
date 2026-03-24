using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CinemaAPI.Data;
using CinemaAPI.Models;
using System.Net.Http.Headers;

namespace CinemaAPI.Tests;

/// <summary>
/// Custom WebApplicationFactory for integration tests using in-memory database
/// </summary>
public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public IntegrationTestFactory()
    {
        _dbName = $"CinemaTestDb_{Guid.NewGuid()}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            dbContext.Database.EnsureCreated();
            SeedTestData(dbContext);
        });
    }

    private static void SeedTestData(DatabaseContext context)
    {
        var adminRole = new Role { Id = 1, Name = "admin", Description = "Administrator" };
        var clientRole = new Role { Id = 2, Name = "client", Description = "Client" };

        context.Roles.AddRange(adminRole, clientRole);

        // Password hashes are SHA256 in Base64 format (as per AuthController.HashPassword)
        var adminUser = new User
        {
            Id = 1,
            Name = "Admin",
            Surname = "Adminov",
            Email = "admin@cinema.com",
            RoleId = 1,
            Login = new Login
            {
                Id = 1,
                LoginValue = "admin",
                PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", // SHA256 Base64("admin")
                UserId = 1
            }
        };

        var clientUser = new User
        {
            Id = 2,
            Name = "John",
            Surname = "Doe",
            Email = "john@example.com",
            RoleId = 2,
            Login = new Login
            {
                Id = 2,
                LoginValue = "johndoe",
                PasswordHash = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", // SHA256 Base64("password")
                UserId = 2
            }
        };

        context.Users.AddRange(adminUser, clientUser);

        var comedyGenre = new Genre { Id = 1, Name = "Comedy", Description = "Funny movies" };
        var actionGenre = new Genre { Id = 2, Name = "Action", Description = "Action movies" };

        context.Genres.AddRange(comedyGenre, actionGenre);

        var film1 = new Film
        {
            Id = 1,
            Name = "Super Comedy",
            Description = "Very funny movie",
            ReleaseDate = DateTime.UtcNow.AddYears(-1),
            GenreId = 1,
            AuthorId = 1,
            ImageUrl = "https://example.com/comedy.jpg"
        };

        var film2 = new Film
        {
            Id = 2,
            Name = "Action Hero",
            Description = "Explosive action",
            ReleaseDate = DateTime.UtcNow.AddYears(-2),
            GenreId = 2,
            AuthorId = 1,
            ImageUrl = "https://example.com/action.jpg"
        };

        context.Films.AddRange(film1, film2);

        context.SaveChanges();
    }

    public HttpClient CreateAuthorizedClient(string role = "admin")
    {
        var client = CreateClient();
        var token = GenerateJwtToken(role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateJwtToken(string role)
    {
        var key = System.Text.Encoding.UTF8.GetBytes("sGfUT7LWQwU7TGB4aEHLDEKhFWst9wNh");
        var issuer = "CinemaAPI";
        var audience = "CinemaAPIUsers";

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, role == "admin" ? "1" : "2"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role),
            new System.Security.Claims.Claim("Email", role == "admin" ? "admin@cinema.com" : "john@example.com"),
            new System.Security.Claims.Claim("Name", role == "admin" ? "Admin" : "John"),
            new System.Security.Claims.Claim("Surname", role == "admin" ? "Adminov" : "Doe")
        };

        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
            Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
