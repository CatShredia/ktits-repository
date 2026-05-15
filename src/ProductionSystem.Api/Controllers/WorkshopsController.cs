using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Director)]
public class WorkshopsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WorkshopDto>>> List(CancellationToken ct)
    {
        var workshops = await db.Workshops.AsNoTracking()
            .Include(w => w.LayoutItems)
            .OrderBy(w => w.Name)
            .ToListAsync(ct);

        return Ok(workshops.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkshopDto>> Get(int id, CancellationToken ct)
    {
        var w = await db.Workshops.AsNoTracking()
            .Include(x => x.LayoutItems)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return w is null ? NotFound() : Ok(Map(w));
    }

    [HttpPut("{id:int}/layout")]
    public async Task<IActionResult> SaveLayout(int id, [FromBody] WorkshopLayoutSaveRequest req, CancellationToken ct)
    {
        var workshop = await db.Workshops.Include(w => w.LayoutItems).FirstOrDefaultAsync(w => w.Id == id, ct);
        if (workshop is null)
            return NotFound();

        foreach (var item in req.Items)
        {
            if (!WorkshopIconTypes.All.Contains(item.IconType))
                return BadRequest(new { message = $"Неизвестный тип значка: {item.IconType}" });
        }

        db.WorkshopLayoutItems.RemoveRange(workshop.LayoutItems);
        workshop.LayoutItems = req.Items.Select(i => new WorkshopLayoutItem
        {
            WorkshopId = id,
            IconType = i.IconType,
            X = i.X,
            Y = i.Y,
        }).ToList();

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static WorkshopDto Map(Workshop w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        FloorPlanBase64 = w.FloorPlanImage is { Length: > 0 }
            ? Convert.ToBase64String(w.FloorPlanImage)
            : null,
        LayoutItems = w.LayoutItems.Select(i => new WorkshopLayoutItemDto
        {
            Id = i.Id,
            IconType = i.IconType,
            X = i.X,
            Y = i.Y,
        }).ToList(),
    };
}
