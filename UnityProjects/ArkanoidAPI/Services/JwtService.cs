using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ArkanoidAPI.Services;

public interface IJwtService
{
    string GenerateToken(int userId, string username, string userIdGuid);
}

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        _secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey not found");
        _issuer = jwtSettings["Issuer"] ?? "ArkanoidAPI";
        _audience = jwtSettings["Audience"] ?? "ArkanoidClient";
        _expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");
    }

    public string GenerateToken(int userId, string username, string userIdGuid)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", userIdGuid)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
