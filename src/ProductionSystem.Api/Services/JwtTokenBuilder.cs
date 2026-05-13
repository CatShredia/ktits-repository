using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProductionSystem.Api.Services;

public class JwtTokenBuilder(IConfiguration config)
{
    public string CreateToken(string login, string role, string? fullName)
    {
        var keyStr = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, login),
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, role),
        };
        if (!string.IsNullOrWhiteSpace(fullName))
            claims.Add(new Claim("fullName", fullName));

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
