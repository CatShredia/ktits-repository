using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArkanoidAPI.Models;
using ArkanoidAPI.Services;

namespace ArkanoidAPI.Controllers;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Получение всех пользователей (требуется авторизация)
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserId = u.UserId,
            Username = u.Username,
            Coins = u.Coins,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        });

        return Ok(userDtos);
    }

    /// <summary>
    /// Получение пользователя по ID (требуется авторизация)
    /// </summary>
    /// <param name="id">ID пользователя</param>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Coins = user.Coins,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    /// <summary>
    /// Получение текущего авторизованного пользователя
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Coins = user.Coins,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }
}
