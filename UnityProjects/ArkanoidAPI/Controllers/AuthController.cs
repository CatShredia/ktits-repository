using Microsoft.AspNetCore.Mvc;
using ArkanoidAPI.Models;
using ArkanoidAPI.Services;

namespace ArkanoidAPI.Controllers;

/// <summary>
/// Контроллер для авторизации и регистрации
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="dto">Данные для регистрации</param>
    /// <returns>JWT токен и информация о пользователе</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
        {
            return BadRequest(new { message = "Пользователь с таким именем уже существует" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Вход пользователя
    /// </summary>
    /// <param name="dto">Данные для входа</param>
    /// <returns>JWT токен и информация о пользователе</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
