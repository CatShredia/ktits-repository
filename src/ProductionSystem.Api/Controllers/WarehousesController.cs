using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{UserRoles.Designer},{UserRoles.Foreman},{UserRoles.Manager},{UserRoles.Director}")]
public class WarehousesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WarehouseDto>>> List(CancellationToken ct)
    {
        var list = await db.Warehouses.AsNoTracking()
            .OrderBy(w => w.Id)
            .Select(w => new WarehouseDto { Id = w.Id, Name = w.Name })
            .ToListAsync(ct);
        return Ok(list);
    }
}
