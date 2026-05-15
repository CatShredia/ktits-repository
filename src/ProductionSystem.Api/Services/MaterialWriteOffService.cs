using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class MaterialWriteOffService(AppDbContext db, ProductRequirementsService requirements)
{
    public async Task<(bool Ok, string? Error)> WriteOffForOrderAsync(string orderNumber, CancellationToken ct)
    {
        var order = await db.CustomerOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Number == orderNumber, ct);
        if (order is null)
            return (false, "Заказ не найден.");

        var (matList, compList) = await requirements.ExplodeAsync(order.ProductName, 1m, ct);
        var materials = matList.ToDictionary(m => m.MaterialId, m => m.Quantity);
        var components = compList.ToDictionary(c => c.ComponentId, c => c.Quantity);

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
}
