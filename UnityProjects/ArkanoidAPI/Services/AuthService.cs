using System.Security.Cryptography;
using System.Text;
using ArkanoidAPI.Database;
using ArkanoidAPI.Database.DTOs;
using ArkanoidAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArkanoidAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto?> LoginAsync(LoginDto dto);

    Task<User?> GetUserByIdAsync(int id);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<IEnumerable<User>> GetAllUsersAsync();
}

public class AuthService : IAuthService
{
    private readonly ArkanoidDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(ArkanoidDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        {
            return null;
        }

        var passwordHash = HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            UserId = Guid.NewGuid().ToString(),
            Coins = 100,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Username, user.UserId);

        return new AuthResponseDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
        {
            return null;
        }

        if (HashPassword(dto.Password) != user.PasswordHash)
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Username, user.UserId);

        return new AuthResponseDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
