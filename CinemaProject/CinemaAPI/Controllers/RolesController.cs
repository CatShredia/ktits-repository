using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers;

// 
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly DatabaseContext _context;

    public RolesController(DatabaseContext context)
    {
        _context = context;
    }

    // ! GetRoles - returns all roles list
    // GET /api/Roles (из CinemaBlazor при загрузке списка ролей)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        return await _context.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync();
    }

    // ! GetRole - returns single role by ID
    // GET /api/Roles/{id} (из CinemaBlazor при загрузке детали роли)
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }

    // ! CreateRole - creates new role (admin only)
    // POST /api/Roles (из CinemaBlazor при создании роли)
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RoleDto>> CreateRole(RoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, dto);
    }

    // ! InitializeDefaultRoles - creates default roles (admin, client) when DB starts
    // POST /api/Roles/initialize (вызывается при инициализации БД)
    [HttpPost("initialize")]
    [AllowAnonymous]
    public async Task<IActionResult> InitializeDefaultRoles()
    {
        if (await _context.Roles.AnyAsync())
        {
            return BadRequest("Roles already exist");
        }

        var roles = new List<Role>
        {
            new Role { Name = "admin", Description = "Администратор - полный доступ ко всем функциям" },
            new Role { Name = "client", Description = "Пользователь - ограниченный доступ, только просмотр и оценки" }
        };

        _context.Roles.AddRange(roles);
        await _context.SaveChangesAsync();

        return Ok("Default roles created");
    }
}
