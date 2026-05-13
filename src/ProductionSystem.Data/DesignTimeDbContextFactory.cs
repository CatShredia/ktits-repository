using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProductionSystem.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                   ?? ResolveConnectionStringFromAppsettings()
                   ?? "Host=localhost;Database=production_system;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new AppDbContext(options);
    }

    private static string? ResolveConnectionStringFromAppsettings()
    {
        foreach (var basePath in GetAppsettingsSearchPaths())
        {
            var path = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(path))
                continue;

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var conn = config.GetConnectionString("Default")
                       ?? config.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(conn))
                return conn;
        }

        return null;
    }

    private static IEnumerable<string> GetAppsettingsSearchPaths()
    {
        var cwd = Directory.GetCurrentDirectory();
        yield return cwd;

        var dir = new DirectoryInfo(cwd);
        while (dir != null)
        {
            var api = Path.Combine(dir.FullName, "src", "ProductionSystem.Api");
            if (Directory.Exists(api))
                yield return api;

            dir = dir.Parent;
        }
    }
}
