using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi3K.Interfaces;
using TestApi3K.Requests;
using TestApi3K.Service;

namespace TestBlazor3K.ApiRequest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUsersLoginsService _userService;

    public AuthController(IConfiguration config, IUsersLoginsService service)
    {
        _config = config;
        _userService = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Login and password are required."
            });
        }

        var user = await _userService.GetUserByLoginAsync(request.Login);

        if (user == null) return NotFound();

        var token = GenerateJwtToken(request.Login, user.id_Role);

        return Ok(new AuthResponse
        {
            Token = token,
            UserName = request.Login,
            Success = true,
            Message = "Login successful"
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] CreateNewUserAndLogin request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Login and password are required."
            });
        }

        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Passwords do not match."
            });
        }

        var existingUser = await _userService.GetUserByLoginAsync(request.Login);
        if (existingUser != null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Login already exists."
            });
        }

        // Создаём пользователя
        var result = await _userService.CreateUserAsync(request);

        if (!result)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Failed to create user."
            });
        }

        var token = GenerateJwtToken(request.Login, request.id_Role);

        return Ok(new AuthResponse
        {
            Token = token,
            UserName = request.Login,
            Success = true,
            Message = "Registration successful"
        });
    }

    private string GenerateJwtToken(string login, int? roleId)
    {
        var secretKey = _config["Jwt:SecretKey"]
            ?? throw new Exception("Jwt:SecretKey not found in configuration");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Определяем роль: если id_Role = 1, то "Admin", иначе "User"
        var roleName = roleId == 1 ? "Admin" : "User";

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, roleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}