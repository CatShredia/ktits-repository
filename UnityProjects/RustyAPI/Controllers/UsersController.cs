using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RustyAPI.Database.DTOs;
using RustyAPI.Services;

namespace RustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var userId = GetAuthorizedUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var profile = await _userService.GetProfileAsync(userId.Value);
        return profile == null ? NotFound() : Ok(profile);
    }

    [HttpPut("me/coins")]
    public async Task<ActionResult<UserProfileDto>> AddCoins([FromBody] UpdateCoinsDto dto)
    {
        var userId = GetAuthorizedUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var profile = await _userService.AddCoinsAsync(userId.Value, dto.CoinsDelta);
        return profile == null ? NotFound() : Ok(profile);
    }

    [HttpPut("me/progress")]
    public async Task<ActionResult<UserProfileDto>> SaveProgress([FromBody] UpdateProgressDto dto)
    {
        var userId = GetAuthorizedUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var profile = await _userService.SaveProgressAsync(userId.Value, dto);
        return profile == null ? NotFound() : Ok(profile);
    }

    private int? GetAuthorizedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return null;
        }

        return userId;
    }
}
