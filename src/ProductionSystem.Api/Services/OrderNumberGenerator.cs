using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public static class OrderNumberGenerator
{
    public static async Task<string> GenerateAsync(AppDbContext db, string customerLogin, DateOnly orderDate, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Login == customerLogin, ct);
        var (lastNameInitial, firstNameInitial) = ParseNameInitials(user.FullName);

        var count = await db.CustomerOrders.CountAsync(o => o.CustomerLogin == customerLogin, ct);
        var seq = (count % 99) + 1;

        var y = orderDate.Year.ToString("D4");
        var m = orderDate.Month.ToString("D2");
        var d = orderDate.Day.ToString("D2");
        var nn = seq.ToString("D2");

        return $"{lastNameInitial}{firstNameInitial}{y}{m}{d}{nn}";
    }

    private static (char Last, char First) ParseNameInitials(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ('_', '_');

        var parts = fullName.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var lastName = parts.Length > 0 ? parts[0].Trim() : "";
        var rest = parts.Length > 1 ? parts[1].Trim() : "";
        var firstName = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

        char li = lastName.Length > 0 ? char.ToUpperInvariant(lastName[0]) : '_';
        char fi = firstName.Length > 0 ? char.ToUpperInvariant(firstName[0]) : '_';
        return (li, fi);
    }
}
