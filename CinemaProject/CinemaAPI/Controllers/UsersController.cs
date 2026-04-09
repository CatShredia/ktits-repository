using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET /api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers(
        [FromQuery] string? search = null,
        [FromQuery] string? searchEmail = null,
        [FromQuery] int? roleId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var users = await _userService.GetAllUsersAsync(search, searchEmail, roleId, sortBy, sortDescending);
        return Ok(users);
    }

    // GET /api/Users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // POST /api/Users
    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> PostUser(UserCreateDto dto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/Users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
    {
        try
        {
            await _userService.UpdateUserAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException)
        {
            return BadRequest("ID mismatch");
        }
    }

    // PUT /api/Users/{id}/login
    [HttpPut("{id}/login")]
    public async Task<IActionResult> PutUserLogin(int id, LoginUpdateSimpleDto dto)
    {
        try
        {
            await _userService.UpdateUserLoginAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST /api/Users/{id}/login
    [HttpPost("{id}/login")]
    public async Task<IActionResult> PostUserLogin(int id, LoginCreateSimpleDto dto)
    {
        try
        {
            await _userService.CreateUserLoginAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE /api/Users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE /api/Users/{id}/login
    [HttpDelete("{id}/login")]
    public async Task<IActionResult> DeleteUserLogin(int id)
    {
        try
        {
            await _userService.DeleteUserLoginAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
