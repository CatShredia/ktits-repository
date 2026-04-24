using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RustyAPI.Database;
using RustyAPI.Database.DTOs;
using RustyAPI.Database.Models;

namespace RustyAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<User?> GetUserByIdAsync(int id);
}

public class AuthService : IAuthService
{
    private readonly RustyDbContext _dbContext;
    private readonly IJwtService _jwtService;

    public AuthService(RustyDbContext dbContext, IJwtService jwtService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == dto.Username))
        {
            return null;
        }

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            Coins = 0,
            LastCompletedLevelIndex = 0,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || user.PasswordHash != HashPassword(dto.Password))
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public Task<User?> GetUserByIdAsync(int id)
    {
        return _dbContext.Users
            .Include(u => u.LevelProgresses)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Token = _jwtService.GenerateToken(user.Id, user.Username, user.UserId),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}
