using Microsoft.AspNetCore.Mvc;
using TestApi3K.Interfaces;
using TestApi3K.Requests;
using TestApi3K.Service;

namespace TestBlazor3K.ApiRequest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUsersLoginsService _userService;

    public AuthController(IUsersLoginsService service)
    {
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

        var user = await _userService.GetUserWithLoginDetailsAsync(request.Login, request.Password);

        if (user == null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid login or password."
            });
        }

        return Ok(new AuthResponse
        {
            UserId = user.id_User,
            UserName = request.Login,
            RoleId = user.id_Role,
            Success = true,
            Message = "Login successful"
        });
    }

    [HttpPost("register")]
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

        var result = await _userService.CreateUserAsync(request);

        if (!result)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Failed to create user."
            });
        }

        var createdUser = await _userService.GetUserByLoginAsync(request.Login);

        return Ok(new AuthResponse
        {
            UserId = createdUser?.id_User ?? 0,
            UserName = request.Login,
            RoleId = request.id_Role,
            Success = true,
            Message = "Registration successful"
        });
    }
}