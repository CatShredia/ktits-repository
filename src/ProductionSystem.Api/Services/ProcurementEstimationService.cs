using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class ProcurementEstimationService(AppDbContext db, ProductRequirementsService requirements)
{
    public sealed record Line(
        string Kind,
        int ItemId,
        string Article,
        string Name,
        string Unit,
        decimal RequiredQuantity,
        decimal AvailableQuantity,
        decimal ShortageQuantity,
        decimal PurchasePrice,
        decimal LineCost,
        int? DeliveryDays);

    public sealed record Result(
        IReadOnlyList<Line> Lines,
        decimal TotalProcurementCost,
        int MinDeliveryDays,
        int MinDeliveryDaysForShortage);

    public async Task<Result> EstimateAsync(string productName, CancellationToken ct)
    {
        var (mats, comps) = await requirements.ExplodeAsync(productName, 1m, ct);
        var lines = new List<Line>();

        foreach (var m in mats)
        {
            var rows = await db.Materials.AsNoTracking()
                .Include(x => x.Supplier)
                .Where(x => x.Article == m.Article)
                .ToListAsync(ct);
            if (rows.Count == 0)
            {
                lines.Add(new Line("Материал", m.MaterialId, m.Article, m.Name, m.Unit,
                    m.Quantity, 0, m.Quantity, 0, 0, null));
                continue;
            }

            var available = rows.Sum(x => x.Quantity);
            var shortage = Math.Max(0, m.Quantity - available);
            var price = rows.First().PurchasePrice;
            var delivery = rows.Where(x => x.Supplier != null).Select(x => x.Supplier!.DeliveryDays).DefaultIfEmpty(0).Max();
            lines.Add(new Line("Материал", m.MaterialId, m.Article, m.Name, m.Unit,
                m.Quantity, available, shortage, price, shortage * price, delivery));
        }

        foreach (var c in comps)
        {
            var rows = await db.Components.AsNoTracking()
                .Include(x => x.Supplier)
                .Where(x => x.Article == c.Article)
                .ToListAsync(ct);
            if (rows.Count == 0)
            {
                lines.Add(new Line("Комплектующее", c.ComponentId, c.Article, c.Name, c.Unit,
                    c.Quantity, 0, c.Quantity, 0, 0, null));
                continue;
            }

            var available = rows.Sum(x => x.Quantity);
            var shortage = Math.Max(0, c.Quantity - available);
            var price = rows.First().PurchasePrice;
            var delivery = rows.Where(x => x.Supplier != null).Select(x => x.Supplier!.DeliveryDays).DefaultIfEmpty(0).Max();
            lines.Add(new Line("Комплектующее", c.ComponentId, c.Article, c.Name, c.Unit,
                c.Quantity, available, shortage, price, shortage * price, delivery));
        }

        var shortageLines = lines.Where(l => l.ShortageQuantity > 0).ToList();
        var minDeliveryAll = lines.Where(l => l.DeliveryDays is int d).Select(l => l.DeliveryDays!.Value).DefaultIfEmpty(0).Max();
        var minDeliveryShortage = shortageLines.Where(l => l.DeliveryDays is int d).Select(l => l.DeliveryDays!.Value).DefaultIfEmpty(0).Max();

        return new Result(
            lines,
            lines.Sum(l => l.LineCost),
            minDeliveryAll,
            minDeliveryShortage);
    }
}
