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

// Register, Login, GenerateJWT, UserProfile get/update, Hash/Unhash password, JWTToken get
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

    // ! Register a new user
    // POST /api/Auth/register (из CinemaBlazor через AuthService.RegisterAsync)
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto dto)
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
            PasswordHash = HashPassword(dto.Password)
        };

        user.Login = login;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var roleName = role?.Name ?? "client";

        // Автоматическое добавление в общие чаты (Group и Channel)
        var commonGroup = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.ConversationType.Name == "Group" && c.Id == 6);
        var commonChannel = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.ConversationType.Name == "Channel" && c.Id == 7);

        var memberRole = await _context.ConversationRoles.FirstOrDefaultAsync(r => r.Name == "Member");
        var moderatorRole = await _context.ConversationRoles.FirstOrDefaultAsync(r => r.Name == "Moderator");

        // Определяем роль в чате: admin → Moderator, остальные → Member
        var chatRole = roleName == "admin" ? moderatorRole : memberRole;

        if (commonGroup != null && chatRole != null)
        {
            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = commonGroup.Id,
                UserId = user.Id,
                RoleId = chatRole.Id
            });
        }

        if (commonChannel != null && memberRole != null)
        {
            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = commonChannel.Id,
                UserId = user.Id,
                RoleId = memberRole.Id
            });
        }

        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user.Id, roleName, user, role);

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
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = roleName
            },
            Role = roleName
        };
    }

    // ! Login user
    // POST /api/Auth/login (из CinemaBlazor через AuthService.LoginAsync)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto dto)
    {
        var login = await _context.Logins
            .Include(l => l.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(l => l.LoginValue == dto.Login);

        if (login == null || !VerifyPassword(dto.Password, login.PasswordHash))
        {
            return Unauthorized("Invalid login or password");
        }

        var roleName = login.User.Role?.Name ?? "client";

        var token = GenerateJwtToken(login.UserId, roleName, login.User, login.User.Role);

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
                Email = login.User.Email,
                RoleId = login.User.RoleId,
                RoleName = roleName
            },
            Role = roleName
        };
    }

    // ! Get profile - returns current authenticated user data
    // GET /api/Auth/me (из CinemaBlazor через AuthService.GetCurrentUserAsync)
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

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
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name
        };
    }

    // !Update profile - updates current user's profile data
    // PUT /api/Auth/me (из CinemaBlazor через AuthService.UpdateProfileAsync)
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

    // ! GenerateJWT - creates JWT token with userId, roleName, name, surname, email claims
    // вызывается внутри Register и Login методов этого контроллера
    private string GenerateJwtToken(int userId, string role, User? user = null, Role? userRole = null)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "sGfUT7LWQwU7TGB4aEHLDEKhFWst9wNh"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

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

    // ! Get userId from JWT token - extracts user ID from claims
    // вызывается внутри всех методов этого контроллера
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    // ! Hash password using SHA256 algorithm
    // вызывается внутри Register и Login методов этого контроллера
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    // ! Verify password by hashing input and comparing with stored hash
    // вызывается внутри Login метода этого контроллера
    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}
