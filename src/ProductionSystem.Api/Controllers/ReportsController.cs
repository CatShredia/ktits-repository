using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(AppDbContext db) : ControllerBase
{
    private const string Roles = $"{UserRoles.Manager},{UserRoles.Director}";

    [HttpGet("inventory")]
    [Authorize(Roles = Roles)]
    public async Task<ActionResult<InventoryReportResponse>> Inventory(
        [FromQuery] string kind,
        [FromQuery] string? type,
        CancellationToken ct)
    {
        var normalized = kind?.Trim().ToLowerInvariant();
        var isMaterials = normalized is "materials" or "материалы" or "material";
        var isComponents = normalized is "components" or "комплектующие" or "component";
        if (!isMaterials && !isComponents)
            return BadRequest(new { message = "Укажите kind=materials или kind=components." });
        var warehouses = await db.Warehouses.AsNoTracking().OrderBy(w => w.Name).ToListAsync(ct);
        var groups = new List<InventoryWarehouseGroupDto>();
        decimal grand = 0;

        foreach (var wh in warehouses)
        {
            if (isMaterials)
            {
                var q = db.Materials.AsNoTracking().Where(m => m.WarehouseId == wh.Id);
                if (!string.IsNullOrWhiteSpace(type) && type != "Все")
                    q = q.Where(m => m.MaterialType == type);

                var lines = await q.OrderBy(m => m.Article).ToListAsync(ct);
                if (lines.Count == 0)
                    continue;

                var total = lines.Sum(l => l.Quantity);
                grand += total;
                groups.Add(new InventoryWarehouseGroupDto
                {
                    WarehouseId = wh.Id,
                    WarehouseName = wh.Name,
                    WarehouseTotalQuantity = total,
                    Lines = lines.Select(l => new InventoryReportLineDto
                    {
                        Article = l.Article,
                        Name = l.Name,
                        Type = l.MaterialType,
                        Unit = l.Unit,
                        Quantity = l.Quantity,
                        PurchasePrice = l.PurchasePrice,
                    }).ToList(),
                });
            }
            else
            {
                var q = db.Components.AsNoTracking().Where(c => c.WarehouseId == wh.Id);
                if (!string.IsNullOrWhiteSpace(type) && type != "Все")
                    q = q.Where(c => c.ComponentType == type);

                var lines = await q.OrderBy(c => c.Article).ToListAsync(ct);
                if (lines.Count == 0)
                    continue;

                var total = lines.Sum(l => l.Quantity);
                grand += total;
                groups.Add(new InventoryWarehouseGroupDto
                {
                    WarehouseId = wh.Id,
                    WarehouseName = wh.Name,
                    WarehouseTotalQuantity = total,
                    Lines = lines.Select(l => new InventoryReportLineDto
                    {
                        Article = l.Article,
                        Name = l.Name,
                        Type = l.ComponentType,
                        Unit = l.Unit,
                        Quantity = l.Quantity,
                        PurchasePrice = l.PurchasePrice,
                    }).ToList(),
                });
            }
        }

        return Ok(new InventoryReportResponse
        {
            Kind = isMaterials ? "Материалы" : "Комплектующие",
            TypeFilter = string.IsNullOrWhiteSpace(type) ? "Все" : type,
            Warehouses = groups,
            GrandTotalQuantity = grand,
        });
    }

    [HttpGet("inventory/types")]
    [Authorize(Roles = Roles)]
    public async Task<ActionResult<List<string>>> InventoryTypes([FromQuery] string kind, CancellationToken ct)
    {
        var normalized = kind?.Trim().ToLowerInvariant();
        var isMaterials = normalized is "materials" or "материалы" or "material";
        if (isMaterials)
        {
            var types = await db.Materials.AsNoTracking()
                .Select(m => m.MaterialType).Distinct().OrderBy(t => t).ToListAsync(ct);
            return Ok(types);
        }

        var compTypes = await db.Components.AsNoTracking()
            .Select(c => c.ComponentType).Distinct().OrderBy(t => t).ToListAsync(ct);
        return Ok(compTypes);
    }
}
