using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext db) : ControllerBase
{
    private const string ReadRoles = $"{UserRoles.Foreman},{UserRoles.Manager},{UserRoles.Director},{UserRoles.Designer}";
    private const string EditRoles = UserRoles.Foreman;

    [HttpGet]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<List<ProductListItemDto>>> List(CancellationToken ct)
    {
        var products = await db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);
        var mat = await db.ProductMaterialSpecs.AsNoTracking()
            .GroupBy(x => x.ProductName).Select(g => new { g.Key, C = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.C, ct);
        var comp = await db.ProductComponentSpecs.AsNoTracking()
            .GroupBy(x => x.ProductName).Select(g => new { g.Key, C = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.C, ct);
        var asm = await db.ProductAssemblySpecs.AsNoTracking()
            .GroupBy(x => x.ParentProductName).Select(g => new { g.Key, C = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.C, ct);
        var ops = await db.ProductOperationSpecs.AsNoTracking()
            .GroupBy(x => x.ProductName).Select(g => new { g.Key, C = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.C, ct);

        return Ok(products.Select(p => new ProductListItemDto
        {
            Name = p.Name,
            Dimensions = p.Dimensions,
            MaterialCount = mat.GetValueOrDefault(p.Name),
            ComponentCount = comp.GetValueOrDefault(p.Name),
            AssemblyCount = asm.GetValueOrDefault(p.Name),
            OperationCount = ops.GetValueOrDefault(p.Name),
        }).ToList());
    }

    [HttpGet("{name}")]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<ProductDetailDto>> Get(string name, CancellationToken ct)
    {
        var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, ct);
        if (p is null)
            return NotFound();

        return Ok(await MapDetailAsync(name, ct));
    }

    [HttpGet("{name}/drawings/{id:int}")]
    [Authorize(Roles = ReadRoles)]
    public async Task<IActionResult> GetDrawingContent(string name, int id, CancellationToken ct)
    {
        var d = await db.ProductDrawings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.ProductName == name, ct);
        if (d?.Content is null or { Length: 0 })
            return NotFound();
        return File(d.Content, "application/octet-stream", $"{d.Title}.bin");
    }

    [HttpPut("{name}")]
    [Authorize(Roles = EditRoles)]
    public async Task<IActionResult> Update(string name, [FromBody] ProductUpdateRequest req, CancellationToken ct)
    {
        var p = await db.Products.FirstOrDefaultAsync(x => x.Name == name, ct);
        if (p is null)
            return NotFound();

        p.Dimensions = req.Dimensions.Trim();

        var existingMeas = await db.ProductMeasurements.Where(x => x.ProductName == name).ToListAsync(ct);
        db.ProductMeasurements.RemoveRange(existingMeas);
        foreach (var m in req.Measurements)
        {
            if (string.IsNullOrWhiteSpace(m.Description))
                continue;
            db.ProductMeasurements.Add(new ProductMeasurement
            {
                ProductName = name,
                Description = m.Description.Trim(),
                Unit = m.Unit.Trim(),
                Value = m.Value,
            });
        }

        await ReplaceSpecsAsync(name, req, ct);
        await db.SaveChangesAsync(ct);
        return Ok(await MapDetailAsync(name, ct));
    }

    [HttpPost("{name}/drawings")]
    [Authorize(Roles = EditRoles)]
    public async Task<ActionResult<ProductDrawingDto>> AddDrawing(
        string name, [FromBody] ProductDrawingCreateRequest req, CancellationToken ct)
    {
        if (!await db.Products.AnyAsync(p => p.Name == name, ct))
            return NotFound();

        byte[]? content = null;
        if (!string.IsNullOrWhiteSpace(req.ContentBase64))
        {
            try { content = Convert.FromBase64String(req.ContentBase64); }
            catch { return BadRequest(new { message = "Некорректные данные чертежа." }); }
        }

        var entity = new ProductDrawing
        {
            ProductName = name,
            Title = req.Title.Trim(),
            Source = string.IsNullOrWhiteSpace(req.Source) ? "Конструктор" : req.Source.Trim(),
            Content = content,
        };
        db.ProductDrawings.Add(entity);
        await db.SaveChangesAsync(ct);

        return Ok(new ProductDrawingDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Source = entity.Source,
            HasContent = content is { Length: > 0 },
        });
    }

    [HttpDelete("{name}/drawings/{id:int}")]
    [Authorize(Roles = EditRoles)]
    public async Task<IActionResult> DeleteDrawing(string name, int id, CancellationToken ct)
    {
        var d = await db.ProductDrawings.FirstOrDefaultAsync(x => x.Id == id && x.ProductName == name, ct);
        if (d is null)
            return NotFound();
        db.ProductDrawings.Remove(d);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("materials-catalog")]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<List<ProductMaterialLineDto>>> MaterialsCatalog(CancellationToken ct) =>
        Ok(await db.Materials.AsNoTracking().OrderBy(m => m.Article)
            .Select(m => new ProductMaterialLineDto
            {
                MaterialId = m.Id,
                Article = m.Article,
                Name = m.Name,
                Unit = m.Unit,
                Quantity = 0,
            }).ToListAsync(ct));

    [HttpGet("components-catalog")]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<List<ProductComponentLineDto>>> ComponentsCatalog(CancellationToken ct) =>
        Ok(await db.Components.AsNoTracking().OrderBy(c => c.Article)
            .Select(c => new ProductComponentLineDto
            {
                ComponentId = c.Id,
                Article = c.Article,
                Name = c.Name,
                Unit = c.Unit,
                Quantity = 0,
            }).ToListAsync(ct));

    [HttpGet("operations-catalog")]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<List<object>>> OperationsCatalog(CancellationToken ct)
    {
        var ops = await db.ProductionOperations.AsNoTracking().OrderBy(o => o.Name).ToListAsync(ct);
        var types = await db.EquipmentTypes.AsNoTracking().OrderBy(t => t.Name).Select(t => t.Name).ToListAsync(ct);
        return Ok(new { operations = ops.Select(o => new { o.Id, o.Name }), equipmentTypes = types });
    }

    private async Task ReplaceSpecsAsync(string name, ProductUpdateRequest req, CancellationToken ct)
    {
        db.ProductMaterialSpecs.RemoveRange(await db.ProductMaterialSpecs.Where(x => x.ProductName == name).ToListAsync(ct));
        db.ProductComponentSpecs.RemoveRange(await db.ProductComponentSpecs.Where(x => x.ProductName == name).ToListAsync(ct));
        db.ProductAssemblySpecs.RemoveRange(await db.ProductAssemblySpecs.Where(x => x.ParentProductName == name).ToListAsync(ct));
        db.ProductOperationSpecs.RemoveRange(await db.ProductOperationSpecs.Where(x => x.ProductName == name).ToListAsync(ct));

        foreach (var m in req.Materials.Where(m => m.MaterialId > 0 && m.Quantity > 0))
        {
            db.ProductMaterialSpecs.Add(new ProductMaterialSpec
            {
                ProductName = name,
                MaterialId = m.MaterialId,
                Quantity = m.Quantity,
            });
        }

        foreach (var c in req.Components.Where(c => c.ComponentId > 0 && c.Quantity > 0))
        {
            db.ProductComponentSpecs.Add(new ProductComponentSpec
            {
                ProductName = name,
                ComponentId = c.ComponentId,
                Quantity = c.Quantity,
            });
        }

        foreach (var a in req.Assemblies.Where(a => !string.IsNullOrWhiteSpace(a.ChildProductName) && a.Quantity > 0))
        {
            if (a.ChildProductName == name)
                continue;
            if (!await db.Products.AnyAsync(p => p.Name == a.ChildProductName, ct))
                continue;
            db.ProductAssemblySpecs.Add(new ProductAssemblySpec
            {
                ParentProductName = name,
                ChildProductName = a.ChildProductName.Trim(),
                Quantity = a.Quantity,
            });
        }

        foreach (var o in req.Operations.Where(o => o.OperationId > 0 && o.DurationMinutes > 0))
        {
            db.ProductOperationSpecs.Add(new ProductOperationSpec
            {
                ProductName = name,
                OperationId = o.OperationId,
                SequenceNumber = o.SequenceNumber,
                EquipmentTypeName = o.EquipmentTypeName,
                DurationMinutes = o.DurationMinutes,
                Description = o.Description,
                RequiresEquipment = o.RequiresEquipment,
            });
        }
    }

    private async Task<ProductDetailDto> MapDetailAsync(string name, CancellationToken ct)
    {
        var p = await db.Products.AsNoTracking().FirstAsync(x => x.Name == name, ct);
        var drawings = await db.ProductDrawings.AsNoTracking().Where(x => x.ProductName == name).ToListAsync(ct);
        var measurements = await db.ProductMeasurements.AsNoTracking().Where(x => x.ProductName == name).ToListAsync(ct);
        var mats = await db.ProductMaterialSpecs.AsNoTracking()
            .Include(x => x.Material).Where(x => x.ProductName == name).ToListAsync(ct);
        var comps = await db.ProductComponentSpecs.AsNoTracking()
            .Include(x => x.Component).Where(x => x.ProductName == name).ToListAsync(ct);
        var assemblies = await db.ProductAssemblySpecs.AsNoTracking()
            .Where(x => x.ParentProductName == name).ToListAsync(ct);
        var ops = await db.ProductOperationSpecs.AsNoTracking()
            .Include(x => x.Operation).Where(x => x.ProductName == name)
            .OrderBy(x => x.SequenceNumber).ToListAsync(ct);

        return new ProductDetailDto
        {
            Name = p.Name,
            Dimensions = p.Dimensions,
            Drawings = drawings.Select(d => new ProductDrawingDto
            {
                Id = d.Id,
                Title = d.Title,
                Source = d.Source,
                HasContent = d.Content is { Length: > 0 },
            }).ToList(),
            Measurements = measurements.Select(m => new ProductMeasurementDto
            {
                Id = m.Id,
                Description = m.Description,
                Unit = m.Unit,
                Value = m.Value,
            }).ToList(),
            Materials = mats.Select(m => new ProductMaterialLineDto
            {
                MaterialId = m.MaterialId,
                Article = m.Material.Article,
                Name = m.Material.Name,
                Unit = m.Material.Unit,
                Quantity = m.Quantity,
            }).ToList(),
            Components = comps.Select(c => new ProductComponentLineDto
            {
                ComponentId = c.ComponentId,
                Article = c.Component.Article,
                Name = c.Component.Name,
                Unit = c.Component.Unit,
                Quantity = c.Quantity,
            }).ToList(),
            Assemblies = assemblies.Select(a => new ProductAssemblyLineDto
            {
                ChildProductName = a.ChildProductName,
                Quantity = a.Quantity,
            }).ToList(),
            Operations = ops.Select(o => new ProductOperationLineDto
            {
                OperationId = o.OperationId,
                OperationName = o.Operation.Name,
                SequenceNumber = o.SequenceNumber,
                EquipmentTypeName = o.EquipmentTypeName,
                DurationMinutes = o.DurationMinutes,
                Description = o.Description,
                RequiresEquipment = o.RequiresEquipment,
            }).ToList(),
        };
    }
}
