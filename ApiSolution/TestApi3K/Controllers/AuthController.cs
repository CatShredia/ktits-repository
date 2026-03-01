using Microsoft.AspNetCore.Mvc;
using TestApi3K.Services.Interfaces;
using TestApi3K.Database.Requests;

namespace TestApi3K.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null || !result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] CreateNewUserAndLogin request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result == null || !result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
