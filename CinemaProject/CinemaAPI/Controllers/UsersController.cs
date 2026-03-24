using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CinemaAPI.Controllers;

/// <summary>
/// Контроллер для управления пользователями (только для администраторов)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;

    public UsersController(DatabaseContext context)
    {
        _context = context;
    }

    // ! Get all users with search, filtering, and sorting
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers(
        [FromQuery] string? search = null,
        [FromQuery] string? searchEmail = null,
        [FromQuery] int? roleId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Include(u => u.Login)
            .AsQueryable();

        // Поиск по фамилии или имени
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Surname.Contains(search) || u.Name.Contains(search));
        }

        // Поиск по email
        if (!string.IsNullOrEmpty(searchEmail))
        {
            query = query.Where(u => u.Email.Contains(searchEmail));
        }

        // Фильтрация по роли
        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

        // Сортировка
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

    // ! Get user by ID with full details
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

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

    // ! Create new user with login
    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> PostUser(UserCreateDto dto)
    {
        // Проверка на существующий login
        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.Login))
        {
            return BadRequest("Login already exists");
        }

        // Проверка на существующий email
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Email already exists");
        }

        // Проверка роли
        Role? role = null;
        if (dto.RoleId.HasValue)
        {
            role = await _context.Roles.FindAsync(dto.RoleId.Value);
        }

        if (role == null)
        {
            role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "client");
        }

        // Создание пользователя
        var user = new User
        {
            Surname = dto.Surname,
            Name = dto.Name,
            Description = dto.Description,
            Gender = dto.Gender,
            Email = dto.Email,
            RoleId = role?.Id
        };

        // Создание логина с хэшированным паролем
        var login = new Login
        {
            LoginValue = dto.Login,
            PasswordHash = HashPassword(dto.Password)
        };

        user.Login = login;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDetailDto
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
        });
    }

    // ! Update user
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Проверка на уникальный email (если email изменён)
        if (user.Email != dto.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
        {
            return BadRequest("Email already exists");
        }

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
            {
                return NotFound("User not found");
            }
            throw;
        }

        return NoContent();
    }

    // ! Update user login (change login value or password)
    [HttpPut("{id}/login")]
    public async Task<IActionResult> PutUserLogin(int id, LoginUpdateSimpleDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.Login == null)
        {
            return NotFound("User has no login");
        }

        // Обновление логина (если предоставлен и изменён)
        if (!string.IsNullOrEmpty(dto.LoginValue) && user.Login.LoginValue != dto.LoginValue)
        {
            if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.LoginValue && l.UserId != id))
            {
                return BadRequest("Login already exists");
            }
            user.Login.LoginValue = dto.LoginValue;
        }

        // Обновление пароля (если предоставлен)
        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Login.PasswordHash = HashPassword(dto.Password);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Logins.Any(e => e.Id == user.Login!.Id))
            {
                return NotFound("Login not found");
            }
            throw;
        }

        return NoContent();
    }

    // ! Delete user (cascade deletes login)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
