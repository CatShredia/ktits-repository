using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/equipment-failures")]
[Authorize(Roles = UserRoles.Foreman)]
public class EquipmentFailuresController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EquipmentFailureDto>>> List(CancellationToken ct)
    {
        var list = await db.EquipmentFailures.AsNoTracking()
            .OrderByDescending(f => f.StartedAt)
            .Select(f => new EquipmentFailureDto
            {
                Id = f.Id,
                EquipmentMarking = f.EquipmentMarking,
                StartedAt = f.StartedAt,
                EndedAt = f.EndedAt,
                Reason = f.Reason,
                RegisteredByLogin = f.RegisteredByLogin,
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentFailureDto>> Create(
        [FromBody] EquipmentFailureCreateRequest req,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.EquipmentMarking))
            return BadRequest(new { message = "Укажите оборудование." });
        if (string.IsNullOrWhiteSpace(req.Reason))
            return BadRequest(new { message = "Укажите причину сбоя." });

        if (!await db.Equipment.AnyAsync(e => e.Marking == req.EquipmentMarking, ct))
            return BadRequest(new { message = "Оборудование не найдено." });

        var login = AuthUserAccessor.GetLogin(User)!;
        var entity = new EquipmentFailure
        {
            EquipmentMarking = req.EquipmentMarking.Trim(),
            StartedAt = req.StartedAt,
            Reason = req.Reason.Trim(),
            RegisteredByLogin = login,
        };
        db.EquipmentFailures.Add(entity);
        await db.SaveChangesAsync(ct);

        return Ok(new EquipmentFailureDto
        {
            Id = entity.Id,
            EquipmentMarking = entity.EquipmentMarking,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            Reason = entity.Reason,
            RegisteredByLogin = entity.RegisteredByLogin,
        });
    }

    [HttpPost("{id:int}/end")]
    public async Task<IActionResult> End(int id, [FromBody] EquipmentFailureEndRequest req, CancellationToken ct)
    {
        var entity = await db.EquipmentFailures.FirstOrDefaultAsync(f => f.Id == id, ct);
        if (entity is null)
            return NotFound();
        if (entity.EndedAt is not null)
            return BadRequest(new { message = "Сбой уже закрыт." });

        entity.EndedAt = req.EndedAt;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
