using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController(AppDbContext db) : ControllerBase
{
    private const string ReadRoles = $"{UserRoles.Designer},{UserRoles.Foreman},{UserRoles.Manager},{UserRoles.Director}";
    private const string ManageRoles = $"{UserRoles.Manager},{UserRoles.Director}";

    [HttpGet]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<MaterialListResponse>> List([FromQuery] int? warehouseId, [FromQuery] bool includeImage = false, CancellationToken ct = default)
    {
        var totalInDb = await db.Materials.CountAsync(ct);

        var query = db.Materials.AsNoTracking().Include(m => m.Supplier).Include(m => m.Warehouse).AsQueryable();
        if (warehouseId is int wid)
            query = query.Where(m => m.WarehouseId == wid);

        var list = await query.OrderBy(m => m.Article).ToListAsync(ct);

        var items = list.Select(m => Map(m, includeImage)).ToList();
        var filteredQty = items.Sum(i => i.Quantity);
        var filteredCost = list.Sum(m => m.Quantity * m.PurchasePrice);

        return Ok(new MaterialListResponse
        {
            Items = items,
            FilteredPositionCount = items.Count,
            FilteredTotalQuantity = filteredQty,
            FilteredTotalPurchaseCost = filteredCost,
            TotalPositionsInDatabase = totalInDb,
        });
    }

    [HttpPut("{article}")]
    [Authorize(Roles = ManageRoles)]
    public async Task<IActionResult> Update(string article, [FromBody] MaterialUpdateRequest req, CancellationToken ct)
    {
        var entity = await db.Materials.FirstOrDefaultAsync(m => m.Article == article, ct);
        if (entity is null)
            return NotFound();

        if (!await db.Warehouses.AnyAsync(w => w.Id == req.WarehouseId, ct))
            return BadRequest(new { message = "Склад не найден." });

        entity.Name = req.Name;
        entity.Unit = req.Unit;
        entity.Quantity = req.Quantity;
        entity.MaterialType = req.MaterialType;
        entity.PurchasePrice = req.PurchasePrice;
        entity.Gost = req.Gost;
        entity.Length = req.Length;
        entity.Characteristics = req.Characteristics;
        entity.WarehouseId = req.WarehouseId;

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{article}")]
    [Authorize(Roles = ManageRoles)]
    public async Task<IActionResult> Delete(string article, CancellationToken ct)
    {
        var entity = await db.Materials.FirstOrDefaultAsync(m => m.Article == article, ct);
        if (entity is null)
            return NotFound();

        if (entity.Quantity != 0)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Удаление возможно только при нулевом количестве на складе." });

        db.Materials.Remove(entity);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static MaterialDto Map(Material m, bool includeImage) => new()
    {
        Article = m.Article,
        Name = m.Name,
        Unit = m.Unit,
        Quantity = m.Quantity,
        SupplierName = m.Supplier?.Name,
        SupplierDeliveryDays = m.Supplier?.DeliveryDays,
        MaterialType = m.MaterialType,
        PurchasePrice = m.PurchasePrice,
        Gost = m.Gost,
        Length = m.Length,
        Characteristics = m.Characteristics,
        WarehouseId = m.WarehouseId,
        WarehouseName = m.Warehouse?.Name,
        ImageBase64 = includeImage && m.Image is { Length: > 0 }
            ? Convert.ToBase64String(m.Image)
            : null,
    };
}
