using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/orders/{orderNumber}/quality-checks")]
[Authorize(Roles = UserRoles.Foreman)]
public class QualityChecksController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<QualityCheckDto>>> List(string orderNumber, CancellationToken ct)
    {
        if (!await db.CustomerOrders.AnyAsync(o => o.Number == orderNumber, ct))
            return NotFound();

        var list = await db.OrderQualityChecks.AsNoTracking()
            .Where(q => q.OrderNumber == orderNumber)
            .OrderBy(q => q.Id)
            .Select(q => new QualityCheckDto
            {
                Id = q.Id,
                OrderNumber = q.OrderNumber,
                ParameterName = q.ParameterName,
                Grade = q.Grade,
                Comment = q.Comment,
                CheckedAt = q.CheckedAt,
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<QualityCheckDto>> Upsert(
        string orderNumber,
        [FromBody] QualityCheckUpsertRequest req,
        CancellationToken ct)
    {
        var order = await db.CustomerOrders.FirstOrDefaultAsync(o => o.Number == orderNumber, ct);
        if (order is null)
            return NotFound();
        if (order.Status != OrderStatuses.QualityControl)
            return BadRequest(new { message = "Контроль качества доступен на этапе «Контроль»." });

        if (string.IsNullOrWhiteSpace(req.ParameterName))
            return BadRequest(new { message = "Укажите параметр." });

        var grade = req.Grade.Trim();
        if (grade is not "+" and not "-")
            return BadRequest(new { message = "Оценка должна быть «+» или «-»." });
        if (grade == "-" && string.IsNullOrWhiteSpace(req.Comment))
            return BadRequest(new { message = "Для отрицательной оценки укажите комментарий." });

        var login = AuthUserAccessor.GetLogin(User)!;
        var existing = await db.OrderQualityChecks.FirstOrDefaultAsync(
            q => q.OrderNumber == orderNumber && q.ParameterName == req.ParameterName.Trim(), ct);

        if (existing is null)
        {
            existing = new OrderQualityCheck
            {
                OrderNumber = orderNumber,
                ParameterName = req.ParameterName.Trim(),
                Grade = grade,
                Comment = req.Comment?.Trim(),
                CheckedAt = DateTime.UtcNow,
                CheckedByLogin = login,
            };
            db.OrderQualityChecks.Add(existing);
        }
        else
        {
            existing.Grade = grade;
            existing.Comment = req.Comment?.Trim();
            existing.CheckedAt = DateTime.UtcNow;
            existing.CheckedByLogin = login;
        }

        await db.SaveChangesAsync(ct);

        return Ok(new QualityCheckDto
        {
            Id = existing.Id,
            OrderNumber = existing.OrderNumber,
            ParameterName = existing.ParameterName,
            Grade = existing.Grade,
            Comment = existing.Comment,
            CheckedAt = existing.CheckedAt,
        });
    }
}
