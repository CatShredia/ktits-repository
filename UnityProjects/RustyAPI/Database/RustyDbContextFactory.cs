using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RustyAPI.Database;

public class RustyDbContextFactory : IDesignTimeDbContextFactory<RustyDbContext>
{
    public RustyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RustyDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rusty_game;Username=postgres;Password=qwerty123");
        return new RustyDbContext(optionsBuilder.Options);
    }
}
