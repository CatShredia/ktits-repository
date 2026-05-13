using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponentsController(AppDbContext db) : ControllerBase
{
    private const string ReadRoles = $"{UserRoles.Designer},{UserRoles.Foreman},{UserRoles.Manager},{UserRoles.Director}";
    private const string ManageRoles = $"{UserRoles.Manager},{UserRoles.Director}";

    [HttpGet]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<ComponentListResponse>> List([FromQuery] int? warehouseId, [FromQuery] bool includeImage = false, CancellationToken ct = default)
    {
        var totalInDb = await db.Components.CountAsync(ct);

        var query = db.Components.AsNoTracking().Include(c => c.Supplier).Include(c => c.Warehouse).AsQueryable();
        if (warehouseId is int wid)
            query = query.Where(c => c.WarehouseId == wid);

        var list = await query.OrderBy(c => c.Article).ToListAsync(ct);

        var items = list.Select(c => Map(c, includeImage)).ToList();
        var filteredQty = items.Sum(i => i.Quantity);
        var filteredCost = list.Sum(c => c.Quantity * c.PurchasePrice);

        return Ok(new ComponentListResponse
        {
            Items = items,
            FilteredPositionCount = items.Count,
            FilteredTotalQuantity = filteredQty,
            FilteredTotalPurchaseCost = filteredCost,
            TotalPositionsInDatabase = totalInDb,
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = ManageRoles)]
    public async Task<IActionResult> Update(int id, [FromBody] ComponentUpdateRequest req, CancellationToken ct)
    {
        var entity = await db.Components.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
            return NotFound();

        if (!await db.Warehouses.AnyAsync(w => w.Id == req.WarehouseId, ct))
            return BadRequest(new { message = "Склад не найден." });

        entity.Name = req.Name;
        entity.Unit = req.Unit;
        entity.Quantity = req.Quantity;
        entity.ComponentType = req.ComponentType;
        entity.PurchasePrice = req.PurchasePrice;
        entity.Weight = req.Weight;
        entity.WarehouseId = req.WarehouseId;

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = ManageRoles)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await db.Components.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
            return NotFound();

        if (entity.Quantity != 0)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Удаление возможно только при нулевом количестве на складе." });

        db.Components.Remove(entity);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static ComponentDto Map(StockComponent c, bool includeImage) => new()
    {
        Id = c.Id,
        Article = c.Article,
        Name = c.Name,
        Unit = c.Unit,
        Quantity = c.Quantity,
        SupplierName = c.Supplier?.Name,
        SupplierDeliveryDays = c.Supplier?.DeliveryDays,
        ComponentType = c.ComponentType,
        PurchasePrice = c.PurchasePrice,
        Weight = c.Weight,
        WarehouseId = c.WarehouseId,
        WarehouseName = c.Warehouse?.Name,
        ImageBase64 = includeImage && c.Image is { Length: > 0 }
            ? Convert.ToBase64String(c.Image)
            : null,
    };
}
