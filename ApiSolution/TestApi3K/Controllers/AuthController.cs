using Microsoft.AspNetCore.Mvc;
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