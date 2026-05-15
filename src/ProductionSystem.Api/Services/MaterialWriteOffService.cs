using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class MaterialWriteOffService(AppDbContext db)
{
    public async Task<(bool Ok, string? Error)> WriteOffForOrderAsync(string orderNumber, CancellationToken ct)
    {
        var order = await db.CustomerOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Number == orderNumber, ct);
        if (order is null)
            return (false, "Заказ не найден.");

        var materials = new Dictionary<int, decimal>();
        var components = new Dictionary<int, decimal>();
        await ExplodeAsync(order.ProductName, 1m, materials, components, new HashSet<string>(), ct);

        foreach (var (materialId, qty) in materials)
        {
            var mat = await db.Materials.FirstOrDefaultAsync(m => m.Id == materialId, ct);
            if (mat is null)
                return (false, $"Материал id={materialId} не найден.");
            if (mat.Quantity < qty)
                return (false, $"Недостаточно материала «{mat.Name}» на складе (нужно {qty}, есть {mat.Quantity}).");
            mat.Quantity -= qty;
        }

        foreach (var (componentId, qty) in components)
        {
            var comp = await db.Components.FirstOrDefaultAsync(c => c.Id == componentId, ct);
            if (comp is null)
                return (false, $"Комплектующее id={componentId} не найдено.");
            if (comp.Quantity < qty)
                return (false, $"Недостаточно комплектующего «{comp.Name}» на складе (нужно {qty}, есть {comp.Quantity}).");
            comp.Quantity -= qty;
        }

        await db.SaveChangesAsync(ct);
        return (true, null);
    }

    private async Task ExplodeAsync(
        string productName,
        decimal multiplier,
        Dictionary<int, decimal> materials,
        Dictionary<int, decimal> components,
        HashSet<string> visited,
        CancellationToken ct)
    {
        if (!visited.Add(productName))
            return;

        var matSpecs = await db.ProductMaterialSpecs.AsNoTracking()
            .Where(s => s.ProductName == productName).ToListAsync(ct);
        foreach (var s in matSpecs)
            materials[s.MaterialId] = materials.GetValueOrDefault(s.MaterialId) + s.Quantity * multiplier;

        var compSpecs = await db.ProductComponentSpecs.AsNoTracking()
            .Where(s => s.ProductName == productName).ToListAsync(ct);
        foreach (var s in compSpecs)
            components[s.ComponentId] = components.GetValueOrDefault(s.ComponentId) + s.Quantity * multiplier;

        var assemblies = await db.ProductAssemblySpecs.AsNoTracking()
            .Where(s => s.ParentProductName == productName).ToListAsync(ct);
        foreach (var a in assemblies)
            await ExplodeAsync(a.ChildProductName, multiplier * a.Quantity, materials, components, visited, ct);
    }
}
