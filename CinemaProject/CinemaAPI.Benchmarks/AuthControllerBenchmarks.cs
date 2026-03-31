using BenchmarkDotNet.Attributes;
using CinemaAPI.Controllers;
using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CinemaAPI.Benchmarks;

[MemoryDiagnoser]
[HtmlExporter, MarkdownExporter]
public class AuthControllerBenchmarks
{
    private AuthController _controller = null!;
    private DatabaseContext _context = null!;
    private IConfiguration _configuration = null!;

    [GlobalSetup]
    public void Setup()
    {
        _context = CreateContext();
        
        // Create in-memory configuration for JWT
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Key", "sGfUT7LWQwU7TGB4aEHLDEKhFWst9wNh"},
            {"Jwt:Issuer", "CinemaAPI"},
            {"Jwt:Audience", "CinemaAPIUsers"}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _controller = new AuthController(_context, _configuration);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    private DatabaseContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"BenchmarkDb_Auth_{Guid.NewGuid()}")
            .Options;

        var context = new DatabaseContext(options);
        SeedData(context);
        return context;
    }

    private void SeedData(DatabaseContext context)
    {
        // Roles
        var adminRole = new Role { Id = 1, Name = "admin", Description = "Administrator" };
        var clientRole = new Role { Id = 2, Name = "client", Description = "Client" };
        context.Roles.AddRange(adminRole, clientRole);

        // Users
        var adminUser = new User
        {
            Id = 1,
            Name = "Admin",
            Surname = "User",
            Email = "admin@cinema.com",
            Description = "Test Admin",
            Gender = "Other",
            RoleId = 1,
            Login = new Login { Id = 1, LoginValue = "admin", PasswordHash = "hashed_password" }
        };

        var clientUser = new User
        {
            Id = 2,
            Name = "John",
            Surname = "Doe",
            Email = "john@cinema.com",
            Description = "Test Client",
            Gender = "Male",
            RoleId = 2,
            Login = new Login { Id = 2, LoginValue = "client", PasswordHash = "hashed_password" }
        };

        context.Users.AddRange(adminUser, clientUser);

        context.SaveChanges();
    }

    // POST /api/Auth/register
    [Benchmark]
    public async Task<bool> RegisterUser()
    {
        var dto = new UserRegisterDto
        {
            Login = $"user_{Guid.NewGuid().ToString().Substring(0, 8)}",
            Password = "testpassword123",
            Email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@cinema.com",
            Name = "Test",
            Surname = "User",
            Description = "Test user for benchmark",
            Gender = "Other"
        };
        var result = await _controller.Register(dto);
        return true;
    }

    // POST /api/Auth/register (duplicate login)
    [Benchmark]
    public async Task<bool> RegisterDuplicateLogin()
    {
        var dto = new UserRegisterDto
        {
            Login = "admin",  // Already exists
            Password = "testpassword123",
            Email = "newuser@cinema.com",
            Name = "New",
            Surname = "User",
            Description = "Test duplicate login",
            Gender = "Other"
        };
        var result = await _controller.Register(dto);
        return true;
    }

    // POST /api/Auth/login (successful)
    [Benchmark]
    public async Task<bool> LoginUser()
    {
        var dto = new UserLoginDto
        {
            Login = "admin",
            Password = "testpassword123"
        };
        var result = await _controller.Login(dto);
        return true;
    }

    // POST /api/Auth/login (invalid credentials)
    [Benchmark]
    public async Task<bool> LoginInvalidUser()
    {
        var dto = new UserLoginDto
        {
            Login = "admin",
            Password = "wrongpassword"
        };
        var result = await _controller.Login(dto);
        return true;
    }

    // POST /api/Auth/login (non-existent user)
    [Benchmark]
    public async Task<bool> LoginNonExistentUser()
    {
        var dto = new UserLoginDto
        {
            Login = "nonexistent",
            Password = "anypassword"
        };
        var result = await _controller.Login(dto);
        return true;
    }

    // Password hashing benchmark
    [Benchmark]
    public string HashPassword()
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("testpassword123"));
        return Convert.ToBase64String(hashedBytes);
    }

    // Password verification benchmark
    [Benchmark]
    public bool VerifyPassword()
    {
        string password = "testpassword123";
        string hash = "hashed_password";
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        var hashedPassword = Convert.ToBase64String(hashedBytes);
        return hashedPassword == hash;
    }
}
