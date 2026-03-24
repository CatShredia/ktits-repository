using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CinemaAPI.Controllers;

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

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Surname.Contains(search) || u.Name.Contains(search));
        }

        if (!string.IsNullOrEmpty(searchEmail))
        {
            query = query.Where(u => u.Email.Contains(searchEmail));
        }

        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

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

    // ! Get user by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<UserListDto>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return new UserListDto
        {
            Id = user.Id,
            Surname = user.Surname,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            LoginValue = user.Login?.LoginValue
        };
    }

    // ! Create new user with login
    [HttpPost]
    public async Task<ActionResult<UserListDto>> PostUser(UserCreateDto dto)
    {
        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.Login))
        {
            return BadRequest("Login already exists");
        }

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Email already exists");
        }

        Role? role = null;
        if (dto.RoleId.HasValue)
        {
            role = await _context.Roles.FindAsync(dto.RoleId.Value);
        }

        if (role == null)
        {
            role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "client");
        }

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
            PasswordHash = HashPassword(dto.Password),
            UserId = user.Id 
        };

        user.Login = login;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserListDto
        {
            Id = user.Id,
            Surname = user.Surname,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = role?.Name,
            LoginValue = login.LoginValue
        });
    }

    // ! Update user
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

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
                return NotFound();
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
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ! Get all logins
    [HttpGet("logins")]
    public async Task<ActionResult<IEnumerable<LoginResponseDto>>> GetLogins()
    {
        var logins = await _context.Logins
            .Include(l => l.User)
            .ToListAsync();

        return logins.Select(l => new LoginResponseDto
        {
            Id = l.Id,
            LoginValue = l.LoginValue,
            UserId = l.UserId,
            UserName = l.User != null ? $"{l.User.Surname} {l.User.Name}" : null
        }).ToList();
    }

    // ! Get login by ID
    [HttpGet("logins/{id}")]
    public async Task<ActionResult<LoginResponseDto>> GetLogin(int id)
    {
        var login = await _context.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (login == null)
        {
            return NotFound();
        }

        return new LoginResponseDto
        {
            Id = login.Id,
            LoginValue = login.LoginValue,
            UserId = login.UserId,
            UserName = login.User != null ? $"{login.User.Surname} {login.User.Name}" : null
        };
    }

    // ! Create new login for user
    [HttpPost("logins")]
    public async Task<ActionResult<LoginResponseDto>> PostLogin(LoginCreateDto dto)
    {
        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.LoginValue))
        {
            return BadRequest("Login already exists");
        }

        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (await _context.Logins.AnyAsync(l => l.UserId == dto.UserId))
        {
            return BadRequest("User already has a login");
        }

        var login = new Login
        {
            LoginValue = dto.LoginValue,
            PasswordHash = HashPassword(dto.Password),
            UserId = dto.UserId
        };

        _context.Logins.Add(login);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLogin), new { id = login.Id }, new LoginResponseDto
        {
            Id = login.Id,
            LoginValue = login.LoginValue,
            UserId = login.UserId,
            UserName = $"{user.Surname} {user.Name}"
        });
    }

    // ! Update login
    [HttpPut("logins/{id}")]
    public async Task<IActionResult> PutLogin(int id, LoginUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var login = await _context.Logins.FindAsync(id);
        if (login == null)
        {
            return NotFound();
        }

        if (login.LoginValue != dto.LoginValue && 
            await _context.Logins.AnyAsync(l => l.LoginValue == dto.LoginValue && l.Id != id))
        {
            return BadRequest("Login already exists");
        }

        login.LoginValue = dto.LoginValue;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            login.PasswordHash = HashPassword(dto.Password);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Logins.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // ! Delete login
    [HttpDelete("logins/{id}")]
    public async Task<IActionResult> DeleteLogin(int id)
    {
        var login = await _context.Logins.FindAsync(id);
        if (login == null)
        {
            return NotFound();
        }

        _context.Logins.Remove(login);
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

public class LoginResponseDto
{
    public int Id { get; set; }
    public string LoginValue { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string? UserName { get; set; }
}
