using CinemaAPI.Data;
using CinemaAPI.Data.Models;
using CinemaAPI.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto> CreateRoleAsync(RoleDto dto);
    Task InitializeDefaultRolesAsync();
}

public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;

    public RoleService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
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

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }

    public async Task<RoleDto> CreateRoleAsync(RoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }

    public async Task InitializeDefaultRolesAsync()
    {
        if (await _context.Roles.AnyAsync())
            throw new InvalidOperationException("Roles already exist");

        var roles = new List<Role>
        {
            new Role { Name = "admin", Description = "Администратор - полный доступ ко всем функциям" },
            new Role { Name = "client", Description = "Пользователь - ограниченный доступ, только просмотр и оценки" }
        };

        _context.Roles.AddRange(roles);
        await _context.SaveChangesAsync();
    }
}
