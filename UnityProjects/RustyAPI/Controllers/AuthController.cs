using Microsoft.AspNetCore.Mvc;
using RustyAPI.Database.DTOs;
using RustyAPI.Services;

namespace RustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
        {
            return BadRequest(new { message = "Пользователь с таким именем уже существует" });
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
        {
            return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
        }

        return Ok(result);
    }
}
