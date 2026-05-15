using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class ProductRequirementsService(AppDbContext db)
{
    public sealed record MaterialNeed(int MaterialId, string Article, string Name, string Unit, decimal Quantity);
    public sealed record ComponentNeed(int ComponentId, string Article, string Name, string Unit, decimal Quantity);

    public async Task<(List<MaterialNeed> Materials, List<ComponentNeed> Components)> ExplodeAsync(
        string productName,
        decimal multiplier,
        CancellationToken ct)
    {
        var materials = new Dictionary<int, decimal>();
        var components = new Dictionary<int, decimal>();
        await ExplodeCoreAsync(productName, multiplier, materials, components, new HashSet<string>(), ct);

        var matIds = materials.Keys.ToList();
        var compIds = components.Keys.ToList();

        var matRows = await db.Materials.AsNoTracking()
            .Where(m => matIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, ct);
        var compRows = await db.Components.AsNoTracking()
            .Where(c => compIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, ct);

        var matList = materials.Select(kv =>
        {
            matRows.TryGetValue(kv.Key, out var m);
            return new MaterialNeed(kv.Key, m?.Article ?? "", m?.Name ?? $"#{kv.Key}", m?.Unit ?? "", kv.Value);
        }).OrderBy(x => x.Article).ToList();

        var compList = components.Select(kv =>
        {
            compRows.TryGetValue(kv.Key, out var c);
            return new ComponentNeed(kv.Key, c?.Article ?? "", c?.Name ?? $"#{kv.Key}", c?.Unit ?? "", kv.Value);
        }).OrderBy(x => x.Article).ToList();

        return (matList, compList);
    }

    private async Task ExplodeCoreAsync(
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
            await ExplodeCoreAsync(a.ChildProductName, multiplier * a.Quantity, materials, components, visited, ct);
    }
}
