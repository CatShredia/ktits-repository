using System.Text.Json;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoService.Data;

// Строка подключения и контекст для миграций.
// Вызывается из AppComposition и команды dotnet ef.
public sealed class AutoServiceDbContextFactory : IDesignTimeDbContextFactory<AutoServiceDbContext>
{
    public const string ConnectionVariable = "AUTOSERVICE_CONNECTION";

    public AutoServiceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AutoServiceDbContext>()
            .UseNpgsql(ResolveConnectionString())
            .Options;
        return new AutoServiceDbContext(options);
    }

    public static string ResolveConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(ConnectionVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment.Trim();

        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                if (TryRead(Path.Combine(directory.FullName, "appsettings.local.json"), out var value))
                    return value;
                if (TryRead(Path.Combine(directory.FullName, "AutoService.Data", "appsettings.local.json"), out value))
                    return value;
                directory = directory.Parent;
            }
        }

        return "Host=localhost;Port=5432;Database=autoservice;Username=postgres";
    }

    private static bool TryRead(string path, out string connectionString)
    {
        connectionString = string.Empty;
        if (!File.Exists(path))
            return false;

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var section)
            || !section.TryGetProperty("AutoService", out var value))
            return false;

        var text = value.GetString();
        if (string.IsNullOrWhiteSpace(text))
            return false;

        connectionString = text.Trim();
        return true;
    }
}
