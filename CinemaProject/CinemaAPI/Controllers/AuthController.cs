using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">User registration data</param>
    /// <returns>Registration result</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto dto)
    {
        // Check if login already exists
        if (await _context.Logins.AnyAsync(l => l.LoginValue == dto.Login))
        {
            return BadRequest("Login already exists");
        }

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Email already exists");
        }

        // Create user
        var user = new User
        {
            Surname = dto.Surname,
            Name = dto.Name,
            Description = dto.Description,
            Gender = dto.Gender,
            Email = dto.Email
        };

        // Create login with hashed password
        var login = new Login
        {
            LoginValue = dto.Login,
            PasswordHash = HashPassword(dto.Password)
        };

        user.Login = login;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = GenerateJwtToken(user.Id, "client", user);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                Description = user.Description,
                Gender = user.Gender,
                Email = user.Email
            },
            Role = "client"
        };
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>JWT token and user info</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto dto)
    {
        var login = await _context.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LoginValue == dto.Login);

        if (login == null || !VerifyPassword(dto.Password, login.PasswordHash))
        {
            return Unauthorized("Invalid login or password");
        }

        // Determine role (you can add Role field to User or Login table)
        var role = login.User.Email.Contains("admin") ? "admin" : "client";

        // Generate JWT token
        var token = GenerateJwtToken(login.UserId, role, login.User);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = login.User.Id,
                Surname = login.User.Surname,
                Name = login.User.Name,
                Description = login.User.Description,
                Gender = login.User.Gender,
                Email = login.User.Email
            },
            Role = role
        };
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    /// <returns>Current user data</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Surname = user.Surname,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender,
            Email = user.Email
        };
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="dto">Updated user data</param>
    /// <returns>Updated user</returns>
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser(UserResponseDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.Surname = dto.Surname;
        user.Name = dto.Name;
        user.Description = dto.Description;
        user.Gender = dto.Gender;
        user.Email = dto.Email;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string GenerateJwtToken(int userId, string role, User? user = null)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "DefaultSecretKeyForDevelopment123!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        // Добавляем имя и фамилию в токен
        if (user != null)
        {
            claims.Add(new Claim("name", user.Name));
            claims.Add(new Claim("surname", user.Surname));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}
