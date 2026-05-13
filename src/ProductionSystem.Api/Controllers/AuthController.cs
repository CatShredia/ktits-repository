using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, JwtTokenBuilder jwt) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == req.Login, ct);
        if (user is null || user.Password != req.Password)
            return Unauthorized(new { message = "Неверный логин или пароль." });

        var token = jwt.CreateToken(user.Login, user.Role, user.FullName);
        return Ok(new AuthResponse(token, user.Login, user.Role, user.FullName));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Укажите логин и пароль." });

        if (!CustomerPasswordRules.TryValidate(req.Password, out var pwdError))
            return BadRequest(new { message = pwdError });

        if (await db.Users.AnyAsync(u => u.Login == req.Login, ct))
            return Conflict(new { message = "Пользователь с таким логином уже существует." });

        var user = new AppUser
        {
            Login = req.Login.Trim(),
            Password = req.Password,
            Role = UserRoles.Customer,
            FullName = string.IsNullOrWhiteSpace(req.FullName) ? null : req.FullName.Trim(),
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var token = jwt.CreateToken(user.Login, user.Role, user.FullName);
        return Ok(new AuthResponse(token, user.Login, user.Role, user.FullName));
    }
}
