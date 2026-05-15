using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public static class WorkshopSeedService
{
    private static readonly (string Name, string FileName)[] Workshops =
    {
        ("Сборочный цех", "Сборочный цех.png"),
        ("Заготовительный цех", "Заготовительный цех.png"),
        ("Покрасочный цех", "Покрасочный цех.png"),
        ("Механический цех", "Механический цех.png"),
        ("Упаковочный цех", "Упаковочный цех.png"),
    };

    public static async Task EnsureSeededAsync(AppDbContext db, IWebHostEnvironment env, CancellationToken ct = default)
    {
        if (await db.Workshops.AnyAsync(ct))
            return;

        var baseDir = FindWorkshopImageDir(env);
        foreach (var (name, file) in Workshops)
        {
            byte[]? image = null;
            if (baseDir is not null)
            {
                var path = Path.Combine(baseDir, file);
                if (File.Exists(path))
                    image = await File.ReadAllBytesAsync(path, ct);
            }

            db.Workshops.Add(new Workshop { Name = name, FloorPlanImage = image });
        }

        await db.SaveChangesAsync(ct);
    }

    private static string? FindWorkshopImageDir(IWebHostEnvironment env)
    {
        var candidates = new[]
        {
            Path.Combine(env.ContentRootPath, "..", "..", "task", "Ресурсы - Сессия 2", "Цеха", "Цеха"),
            Path.Combine(env.ContentRootPath, "..", "..", "task", "Ресурсы - Сессия 2", "Цеха"),
        };

        foreach (var c in candidates)
        {
            var full = Path.GetFullPath(c);
            if (Directory.Exists(full))
                return full;
        }

        return null;
    }
}
