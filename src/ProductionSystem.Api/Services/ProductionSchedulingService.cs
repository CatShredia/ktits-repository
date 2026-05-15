using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class ProductionSchedulingService(AppDbContext db)
{
    public sealed record OpSpec(
        int OperationId,
        string OperationName,
        int SequenceNumber,
        string? EquipmentTypeName,
        int DurationMinutes,
        string? Description,
        bool RequiresEquipment);

    public sealed record GanttBar(
        string ProductName,
        string OperationName,
        string? EquipmentTypeName,
        string? EquipmentMarking,
        int StartMinutes,
        int EndMinutes,
        bool IsBackground);

    public sealed record ScheduleResult(
        IReadOnlyList<GanttBar> Bars,
        IReadOnlyList<string> EquipmentUsed,
        int ProductionMinutes,
        int BackgroundMinutes);

    private sealed class Graph
    {
        public Dictionary<string, List<(string Child, decimal Qty)>> Assemblies { get; } = new();
        public Dictionary<string, List<OpSpec>> Operations { get; } = new();
    }

    public async Task<ScheduleResult> ScheduleAsync(string rootProductName, CancellationToken ct)
    {
        var graph = await LoadGraphAsync(ct);
        if (!graph.Operations.ContainsKey(rootProductName))
            return new ScheduleResult([], [], 0, 0);

        var equipment = await db.Equipment.AsNoTracking()
            .OrderBy(e => e.Marking)
            .ToListAsync(ct);

        var freeAt = equipment.ToDictionary(e => e.Marking, _ => 0.0);
        var bars = new List<GanttBar>();
        var path = new HashSet<string>();

        var end = ScheduleProduct(rootProductName, graph, equipment, freeAt, bars, path);
        var used = bars.Where(b => b.EquipmentMarking != null)
            .Select(b => b.EquipmentMarking!)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var blocking = bars.Where(b => !b.IsBackground).Select(b => b.EndMinutes).DefaultIfEmpty(0).Max();
        var background = bars.Where(b => b.IsBackground).Sum(b => b.EndMinutes - b.StartMinutes);

        return new ScheduleResult(bars, used, blocking, background);
    }

    private async Task<Graph> LoadGraphAsync(CancellationToken ct)
    {
        var graph = new Graph();
        var assemblies = await db.ProductAssemblySpecs.AsNoTracking().ToListAsync(ct);
        foreach (var a in assemblies)
        {
            if (!graph.Assemblies.TryGetValue(a.ParentProductName, out var list))
                graph.Assemblies[a.ParentProductName] = list = [];
            list.Add((a.ChildProductName, a.Quantity));
        }

        var ops = await db.ProductOperationSpecs.AsNoTracking()
            .Include(o => o.Operation)
            .ToListAsync(ct);
        foreach (var o in ops)
        {
            if (!graph.Operations.TryGetValue(o.ProductName, out var list))
                graph.Operations[o.ProductName] = list = [];
            list.Add(new OpSpec(
                o.OperationId,
                o.Operation.Name,
                o.SequenceNumber,
                o.EquipmentTypeName,
                o.DurationMinutes,
                o.Description,
                o.RequiresEquipment));
        }

        foreach (var kv in graph.Operations)
            graph.Operations[kv.Key] = kv.Value.OrderBy(x => x.SequenceNumber).ToList();

        return graph;
    }

    private static int ScheduleProduct(
        string productName,
        Graph graph,
        List<Equipment> equipment,
        Dictionary<string, double> freeAt,
        List<GanttBar> bars,
        HashSet<string> path)
    {
        if (!path.Add(productName))
            return 0;

        var startTime = 0;
        if (graph.Assemblies.TryGetValue(productName, out var children) && children.Count > 0)
        {
            var childEnds = new List<int>();
            var ordered = children
                .Select(c => (c.Child, c.Qty, EstimateSubtreeMinutes(c.Child, graph)))
                .OrderByDescending(x => x.Item3)
                .ToList();

            foreach (var (child, qty, _) in ordered)
            {
                var copies = (int)Math.Ceiling(qty);
                var copyEnds = new List<int>();
                for (var i = 0; i < copies; i++)
                {
                    var childPath = new HashSet<string>(path);
                    copyEnds.Add(ScheduleProduct(child, graph, equipment, freeAt, bars, childPath));
                }

                childEnds.Add(copyEnds.DefaultIfEmpty(0).Max());
            }

            startTime = childEnds.DefaultIfEmpty(0).Max();
        }

        if (graph.Operations.TryGetValue(productName, out var ops))
        {
            var t = (double)startTime;
            foreach (var op in ops)
            {
                if (!op.RequiresEquipment)
                {
                    var end = (int)Math.Ceiling(t + op.DurationMinutes);
                    bars.Add(new GanttBar(
                        productName, op.OperationName, op.EquipmentTypeName, null,
                        (int)Math.Ceiling(t), end, true));
                    t = end;
                    continue;
                }

                var marking = PickEquipment(op.EquipmentTypeName, t, equipment, freeAt);
                var start = Math.Max(t, freeAt.GetValueOrDefault(marking, 0));
                var finish = start + op.DurationMinutes;
                freeAt[marking] = finish;
                bars.Add(new GanttBar(
                    productName, op.OperationName, op.EquipmentTypeName, marking,
                    (int)Math.Ceiling(start), (int)Math.Ceiling(finish), false));
                t = finish;
            }

            startTime = (int)Math.Ceiling(t);
        }

        path.Remove(productName);
        return startTime;
    }

    private static string PickEquipment(
        string? equipmentType,
        double notBefore,
        List<Equipment> equipment,
        Dictionary<string, double> freeAt)
    {
        var candidates = string.IsNullOrWhiteSpace(equipmentType)
            ? equipment
            : equipment.Where(e => e.EquipmentTypeName == equipmentType).ToList();

        if (candidates.Count == 0)
        {
            var synthetic = $"Виртуальное-{equipmentType ?? "общее"}";
            if (!freeAt.ContainsKey(synthetic))
                freeAt[synthetic] = 0;
            return synthetic;
        }

        return candidates
            .OrderBy(e => Math.Max(notBefore, freeAt.GetValueOrDefault(e.Marking, 0)))
            .ThenBy(e => e.Marking)
            .First()
            .Marking;
    }

    private static int EstimateSubtreeMinutes(string productName, Graph graph)
    {
        var path = new HashSet<string>();
        return EstimateCore(productName, graph, path);
    }

    private static int EstimateCore(string productName, Graph graph, HashSet<string> path)
    {
        if (!path.Add(productName))
            return 0;

        var childParallel = 0;
        if (graph.Assemblies.TryGetValue(productName, out var children))
        {
            var ordered = children
                .Select(c => (c.Child, c.Qty, EstimateCore(c.Child, graph, new HashSet<string>(path))))
                .OrderByDescending(x => x.Item3)
                .ToList();
            foreach (var (child, qty, _) in ordered)
            {
                var copies = (int)Math.Ceiling(qty);
                var maxCopy = 0;
                for (var i = 0; i < copies; i++)
                    maxCopy = Math.Max(maxCopy, EstimateCore(child, graph, new HashSet<string>(path)));
                childParallel = Math.Max(childParallel, maxCopy);
            }
        }

        var opsSum = graph.Operations.TryGetValue(productName, out var ops)
            ? ops.Sum(o => o.DurationMinutes)
            : 0;

        path.Remove(productName);
        return childParallel + opsSum;
    }
}
