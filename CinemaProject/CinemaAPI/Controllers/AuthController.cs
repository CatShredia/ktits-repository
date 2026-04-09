using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AuthController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // POST /api/Auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto dto)
    {
        try
        {
            var result = await _accountService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST /api/Auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto dto)
    {
        try
        {
            var result = await _accountService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid login or password");
        }
    }

    // GET /api/Auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _accountService.GetCurrentUserAsync(userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PUT /api/Auth/me
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser(UserResponseDto dto)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _accountService.UpdateUserProfileAsync(userId.Value, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
