using Microsoft.AspNetCore.Mvc;
using TestApi3K.Services.Interfaces;
using TestApi3K.Database.Models;
using TestApi3K.Database.Requests;

namespace TestApi3K.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userService;

    public AuthService(IUserRepository userService)
    {
        _userService = userService;
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

        var user = await _userService.GetUserWithLoginDetailsAsync(request.Login, request.Password);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid login or password."
            };
        }

        return new AuthResponse
        {
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

        var existingUser = await _userService.GetUserByLoginAsync(request.Login);
        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Login already exists."
            };
        }

        var result = await _userService.CreateUserAsync(request);

        if (!result)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Failed to create user."
            };
        }

        var createdUser = await _userService.GetUserByLoginAsync(request.Login);

        return new AuthResponse
        {
            UserId = createdUser?.id_User ?? 0,
            UserName = request.Login,
            RoleId = request.id_Role,
            Success = true,
            Message = "Registration successful"
        };
    }
}
