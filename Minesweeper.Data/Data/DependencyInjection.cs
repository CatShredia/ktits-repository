using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Data.Repositories;
using Minesweeper.Data.Services;

namespace Minesweeper.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string dbPath = "minesweeper.sqlite")
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite($"Data Source={dbPath}"));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}