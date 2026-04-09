using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET /api/Roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    // GET /api/Roles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    // POST /api/Roles
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RoleDto>> CreateRole(RoleDto dto)
    {
        var role = await _roleService.CreateRoleAsync(dto);
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, dto);
    }

    // POST /api/Roles/initialize
    [HttpPost("initialize")]
    [AllowAnonymous]
    public async Task<IActionResult> InitializeDefaultRoles()
    {
        try
        {
            await _roleService.InitializeDefaultRolesAsync();
            return Ok("Default roles created");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
