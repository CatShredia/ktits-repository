using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TestApi3K.Database.Models;
using TestApi3K.Database.Requests;
using TestApi3K.Services.Interfaces;

namespace TestApi3K.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Login and password are required."
            };
        }

        var user = await _userRepository.GetUserWithLoginDetailsAsync(request.Login, request.Password);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid login or password."
            };
        }

        var token = GenerateJwtToken(request.Login, user.id_Role);

        return new AuthResponse
        {
            Token = token,
            UserId = user.id_User,
            UserName = request.Login,
            RoleId = user.id_Role,
            Success = true,
            Message = "Login successful"
        };
    }

    public async Task<AuthResponse?> RegisterAsync(CreateNewUserAndLogin request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Login and password are required."
            };
        }

        var existingUser = await _userRepository.GetUserByLoginAsync(request.Login);
        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Login already exists."
            };
        }

        if (request.Password != request.Password)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Passwords do not match."
            };
        }

        var result = await _userRepository.CreateUserAsync(request);

        if (!result)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Failed to create user."
            };
        }

        var createdUser = await _userRepository.GetUserByLoginAsync(request.Login);

        var token = GenerateJwtToken(request.Login, request.id_Role);

        return new AuthResponse
        {
            Token = token,
            UserId = createdUser?.id_User ?? 0,
            UserName = request.Login,
            RoleId = request.id_Role,
            Success = true,
            Message = "Registration successful"
        };
    }

    private string GenerateJwtToken(string login, int? roleId)
    {
        var secretKey = _config["Jwt:SecretKey"]
            ?? throw new Exception("Jwt:SecretKey not found in configuration");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
