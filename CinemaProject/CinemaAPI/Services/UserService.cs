using CinemaAPI.Data;
using CinemaAPI.Data.Models;
using CinemaAPI.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CinemaAPI.Services;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetAllUsersAsync(string? search, string? searchEmail, int? roleId, string? sortBy, bool sortDescending);
    Task<UserDetailDto?> GetUserByIdAsync(int id);
    Task<UserDetailDto> CreateUserAsync(UserCreateDto dto);
    Task UpdateUserAsync(int id, UserUpdateDto dto);
    Task UpdateUserLoginAsync(int userId, LoginUpdateSimpleDto dto);
    Task CreateUserLoginAsync(int userId, LoginCreateSimpleDto dto);
    Task DeleteUserAsync(int id);
    Task DeleteUserLoginAsync(int userId);
    string HashPassword(string password);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _context;

    public UserService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserListDto>> GetAllUsersAsync(string? search, string? searchEmail, int? roleId, string? sortBy, bool sortDescending)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Include(u => u.Login)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.Surname.Contains(search) || u.Name.Contains(search));

        if (!string.IsNullOrEmpty(searchEmail))
            query = query.Where(u => u.Email.Contains(searchEmail));

        if (roleId.HasValue)
            query = query.Where(u => u.RoleId == roleId.Value);

        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
            "surname" => sortDescending ? query.OrderByDescending(u => u.Surname) : query.OrderBy(u => u.Surname),
            "email" => sortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "role" => sortDescending
                ? query.OrderByDescending(u => u.Role != null ? u.Role.Name : "")
                : query.OrderBy(u => u.Role != null ? u.Role.Name : ""),
            "id" => sortDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
            _ => query.OrderBy(u => u.Surname)
        };

        var users = await query.ToListAsync();

        return users.Select(u => new UserListDto
        {
            Id = u.Id,
            Surname = u.Surname,
            Name = u.Name,
            Description = u.Description,
            Gender = u.Gender,
            Email = u.Email,
            RoleId = u.RoleId,
            RoleName = u.Role?.Name,
            LoginValue = u.Login?.LoginValue
        }).ToList();
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return new UserDetailDto
        {
            Id = user.Id,
            Surname = user.Surname,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            Login = user.Login != null ? new LoginInfoDto
            {
                Id = user.Login.Id,
                LoginValue = user.Login.LoginValue
            } : null
        };
    }

    public async Task<UserDetailDto> CreateUserAsync(UserCreateDto dto)
    {
        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.Login))
            throw new InvalidOperationException("Login already exists");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already exists");

        Role? role = null;
        if (dto.RoleId.HasValue)
            role = await _context.Roles.FindAsync(dto.RoleId.Value);

        if (role == null)
            role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "client");

        var user = new User
        {
            Surname = dto.Surname,
            Name = dto.Name,
            Description = dto.Description,
            Gender = dto.Gender,
            Email = dto.Email,
            RoleId = role?.Id
        };

        var login = new Login
        {
            LoginValue = dto.Login,
            PasswordHash = HashPassword(dto.Password)
        };

        user.Login = login;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDetailDto
        {
            Id = user.Id,
            Surname = user.Surname,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = role?.Name,
            Login = new LoginInfoDto
            {
                Id = login.Id,
                LoginValue = login.LoginValue
            }
        };
    }

    public async Task UpdateUserAsync(int id, UserUpdateDto dto)
    {
        if (id != dto.Id)
            throw new ArgumentException("ID mismatch");

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (user.Email != dto.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            throw new InvalidOperationException("Email already exists");

        user.Surname = dto.Surname;
        user.Name = dto.Name;
        user.Description = dto.Description;
        user.Gender = dto.Gender;
        user.Email = dto.Email;
        user.RoleId = dto.RoleId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(e => e.Id == id))
                throw new KeyNotFoundException("User not found");
            throw;
        }
    }

    public async Task UpdateUserLoginAsync(int userId, LoginUpdateSimpleDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (user.Login == null)
            throw new KeyNotFoundException("User has no login");

        if (!string.IsNullOrEmpty(dto.LoginValue) && user.Login.LoginValue != dto.LoginValue)
        {
            if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.LoginValue && l.UserId != userId))
                throw new InvalidOperationException("Login already exists");
            user.Login.LoginValue = dto.LoginValue;
        }

        if (!string.IsNullOrEmpty(dto.Password))
            user.Login.PasswordHash = HashPassword(dto.Password);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Logins.Any(e => e.Id == user.Login!.Id))
                throw new KeyNotFoundException("Login not found");
            throw;
        }
    }

    public async Task CreateUserLoginAsync(int userId, LoginCreateSimpleDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (await _context.Logins.AnyAsync(l => l.UserId == userId))
            throw new InvalidOperationException("User already has a login");

        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.LoginValue))
            throw new InvalidOperationException("Login already exists");

        var login = new Login
        {
            LoginValue = dto.LoginValue,
            PasswordHash = HashPassword(dto.Password),
            UserId = userId
        };

        _context.Logins.Add(login);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserLoginAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (user.Login == null)
            throw new KeyNotFoundException("User has no login");

        _context.Logins.Remove(user.Login);
        await _context.SaveChangesAsync();
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
