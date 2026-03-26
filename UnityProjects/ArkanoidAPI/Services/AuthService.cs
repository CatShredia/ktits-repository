using System.Security.Cryptography;
using System.Text;
using ArkanoidAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArkanoidAPI.Services;

/// <summary>
/// Сервис для работы с пользователями и авторизацией
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Вход пользователя
    /// </summary>
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);

    /// <summary>
    /// Получение пользователя по ID
    /// </summary>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Получение пользователя по имени
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Получение всех пользователей
    /// </summary>
    Task<IEnumerable<User>> GetAllUsersAsync();
}

/// <summary>
/// Реализация сервиса авторизации
/// </summary>
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
        // Проверка на существующего пользователя
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        {
            return null;
        }

        // Хэширование пароля
        var passwordHash = HashPassword(dto.Password);

        // Создание пользователя
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

        // Генерация токена
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
        // Поиск пользователя
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
        {
            return null;
        }

        // Проверка пароля
        if (HashPassword(dto.Password) != user.PasswordHash)
        {
            return null;
        }

        // Обновление времени последнего входа
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Генерация токена
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
