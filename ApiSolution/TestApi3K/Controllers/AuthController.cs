using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi3K.Requests;

namespace TestBlazor3K.ApiRequest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    // private readonly UserService _userService; // Инжекция сервиса для работы с БД

    public AuthController(IConfiguration config) // , UserService userService
    {
        _config = config;
        // _userService = userService;
    }

    [HttpPost("login")]
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

        // TODO: ЗАМЕНИТЬ ЭТУ ЧАСТЬ НА РЕАЛЬНУЮ ПРОВЕРКУ В БАЗЕ ДАННЫХ
        // Пример: var user = await _userService.GetUserByLoginAsync(request.Login);
        // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) ...
        
        // Для теста хардкодим пользователя (удалите это в продакшене!)
        if (request.Login != "admin" || request.Password != "12345")
        {
            return Unauthorized(new AuthResponse 
            { 
                Success = false, 
                Message = "Invalid login or password." 
            });
        }

        // Генерация токена
        var token = GenerateJwtToken(request.Login);

        return Ok(new AuthResponse
        {
            Token = token,
            UserName = request.Login,
            Success = true,
            Message = "Login successful"
        });
    }

    private string GenerateJwtToken(string login)
    {
        // Получаем секретный ключ из appsettings.json (см. шаг 4)
        var secretKey = _config["Jwt:SecretKey"] 
            ?? throw new Exception("Jwt:SecretKey not found in configuration");
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Формируем Claims (данные внутри токена)
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, "User"), // Можно динамически брать роль из БД
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2), // Время жизни токена
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}