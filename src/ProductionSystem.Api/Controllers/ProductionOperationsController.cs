using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/production-operations")]
[Authorize(Roles = UserRoles.Director)]
public class ProductionOperationsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductionOperationDto>>> List(CancellationToken ct)
    {
        var list = await db.ProductionOperations.AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new ProductionOperationDto { Id = o.Id, Name = o.Name })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<ProductionOperationDto>> Create([FromBody] ProductionOperationDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Name))
            return BadRequest(new { message = "Укажите название операции." });

        var name = body.Name.Trim();
        if (await db.ProductionOperations.AnyAsync(o => o.Name == name, ct))
            return Conflict(new { message = "Такая операция уже существует." });

        var op = new ProductionOperation { Name = name };
        db.ProductionOperations.Add(op);
        await db.SaveChangesAsync(ct);
        return Ok(new ProductionOperationDto { Id = op.Id, Name = op.Name });
    }
}
