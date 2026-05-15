using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{UserRoles.Foreman},{UserRoles.Director}")]
public class EquipmentController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<object>>> List(CancellationToken ct)
    {
        var list = await db.Equipment.AsNoTracking()
            .OrderBy(e => e.Marking)
            .Select(e => new { e.Marking, e.EquipmentTypeName, e.Characteristics })
            .ToListAsync(ct);
        return Ok(list);
    }
}
