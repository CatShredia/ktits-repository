using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Data.Repositories;
using Minesweeper.Data.Services;

namespace Minesweeper.Data;

public static class DependencyInjection
{
    public const string ConnectionStringVariable = "MINESWEEPER_DB";

    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=minesweeper;Username=postgres;Password=qwerty123";

    public static IServiceCollection AddDataLayer(this IServiceCollection services, string? connectionString = null)
    {
        connectionString ??= Environment.GetEnvironmentVariable(ConnectionStringVariable);
        connectionString = string.IsNullOrWhiteSpace(connectionString) ? DefaultConnectionString : connectionString;

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
